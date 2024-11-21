const mysql = require('mysql')  // 引入mysql插件
const config = require('../config/default')  // 获取链接到数据库需要的参数


/*
// 初次运行会自动创建数据库，不链接到某一个具体的数据库-【改为连接到指定的数据库】
const db = mysql.createConnection({
    host: config.database.HOST, 
    user: config.database.USER,
    password: config.database.PASSWORD,
})

// 链接到指定的数据库
const pool = mysql.createPool({
    host: config.database.HOST, 
    user: config.database.USER,
    password: config.database.PASSWORD,
    database: config.database.WALL, 
})   

*/


// 链接到指定的数据库
const pool = mysql.createPool({
    host: config.database.HOST,
    user: config.database.USER,
    password: config.database.PASSWORD,
    database: config.database.WALL,  // 直接使用已存在的数据库
    port: 3306,
});



/*
// 第一次创建db - 直接使用 —— 使用mysql的方法1
let bdbs = (sql, values) => { // sql为语句，values为对应的参数
    return new Promise((resolve, reject)=>{
        db.query(sql, values, (err,result) => {
            if (err) {
                reject(err)
            }
            else {
                resolve(result)  // 无错，直接返回result
            }
        })
    })
}
*/

// 通过pool.getConnection 获取链接，除了第一次创建数据库，其他都是用这个 —— 使用mysql的方法2
let query = (sql, values) => {
    return new Promise((resolve, reject) => {
        pool.getConnection((err, connection) => {
            if (err) {
                console.error('Error connecting to database:', err);
                reject(err)
            }
            else {
                connection.query(sql, values, (err, rows) => {
                    if (err) {
                        reject(err)
                    }
                    else {
                        resolve(rows)
                    }
                    connection.release();  // 释放该链接，把该链接放回池里供其他人使用
                })
            }
        })
    })
} 


/*
// 创建数据库sql语句
let WALLTS = `create database if not exists WALLTS default charset utf8 collate utf8_general_ci;`

let createDatabase = (db) => {  // 运行之可以创建
    return bdbs(db, [])
} 

    

// 创建数据表 - 留言/相片
let walls = `create table if not exists walls(
    id INT NOT NULL AUTO_INCREMENT COMMENT 'id自动增长',
    classId INT COMMENT '班级id-加!!!!!!!!',
    type INT NOT NULL COMMENT '类型0留言1相片',
    message VARCHAR(1000) COMMENT '留言(相片可为空)',
    name VARCHAR(100) NOT NULL COMMENT '用户名',
    userId VARCHAR(100) NOT NULL COMMENT '创建者ID',
    moment VARCHAR(100) NOT NULL COMMENT '时间',
    label INT NOT NULL COMMENT '标签',
    color INT COMMENT '颜色',
    imgurl VARCHAR(100) COMMENT '图片路径',
    PRIMARY KEY (id)
);`

// 创建数据表 - 留言反馈
let feedbacks = `create table if not exists feedbacks(
    id INT NOT NULL AUTO_INCREMENT,
    wallId INT NOT NULL COMMENT '对应留言的ID',
    userId VARCHAR(100) NOT NULL COMMENT '反馈者ID-游客直接记录ip地址',
    type INT NOT NULL COMMENT '反馈类型0喜欢1举报2撤销',
    moment VARCHAR(100) NOT NULL COMMENT '时间',
    PRIMARY KEY (id)
);`

// 创建数据表 - 留言评论
let comments = `create table if not exists comments(
    id INT NOT NULL AUTO_INCREMENT,
    wallId INT NOT NULL COMMENT '对应留言的ID',
    userId VARCHAR(100) NOT NULL COMMENT '评论者ID',
    imgurl VARCHAR(100) COMMENT '头像路径',
    comment VARCHAR(1000) COMMENT '评论内容',
    name VARCHAR(100) NOT NULL COMMENT '用户名',
    moment VARCHAR(100) NOT NULL COMMENT '时间',
    PRIMARY KEY (id)
);`

let createTable = (sql) => {  // 使用query来创建表
    return query(sql, [])
}

// 先创建数据库再创建数据表
async function create() {
    await createDatabase(WALLTS); // 先等待db创建
    // await initDatabase();
    createTable(walls);
    createTable(feedbacks);
    createTable(comments);
}

create(); // 先改写数据库连接方式，确保创建数据库后再尝试连接到指定数据库

*/

// 新建walls-classId加!!!!!!!!
exports.insertWall = (value) => {
    let _sql = "insert into walls set type=?, message=?, name=?, userId=?, moment=?, label=?, color=?, imgurl=?, classId=?;"
    return query(_sql, value);
}

// 新建反馈
exports.insertFeedback = (value) => {
    let _sql = "insert into feedbacks set wallId=?, userId=?, type=?, moment=?;"
    return query(_sql, value);
}

// 新建评论
exports.insertComment = (value) => {
    let _sql = "insert into comments set wallId=?, userId=?, imgurl=?, moment=?, comment=?, name=?;"
    return query(_sql, value);
}

// 删除墙，主表对应多条子表一起删除
exports.deleteWall = (id) => {
    let _sql = `delete a,b,c from walls a left join feedbacks b on a.id=b.wallId left join comments c on a.id=c.wallId where a.id="${id}";`
    return query(_sql);
}

// 删除反馈
exports.deleteFeedback = (id) => {
    let _sql = `delete from feedbacks where id="${id}";`
    return query(_sql);
}

// 删除评论
exports.deleteComment = (id) => {
    let _sql = `delete from comments where id="${id}";`
    return query(_sql);
}

// 分页查询墙 - pagesize 一页的数量；label 是二级标签的查询；classId加!!!!!!!!
exports.findWallPage = (page, pagesize, type, label, classId) => {
    let _sql;
    if (label == -1) { // 全部查找
        _sql = `select * from walls where type="${type}" and classId="${classId}" order by id desc limit ${(page-1)*pagesize}, ${pagesize};`
    }
    else {
        _sql = `select * from walls where type="${type}" and classId="${classId}" and label="${label}" order by id desc limit ${(page-1)*pagesize}, ${pagesize};`
    }
    return query(_sql);
}

// 倒序分页查询墙的评论
exports.findCommentPage = (page, pagesize, id) => {
    let _sql = `select * from comments where wallId="${id}" order by id desc limit ${(page-1)*pagesize}, ${pagesize};`
    return query(_sql);
}

// 查询各反馈总数据 - 点赞等
exports.feedbackCount = (wid, type) => {
    let _sql = `select count(*) as count from feedbacks where wallId="${wid}" and type="${type}";`
    return query(_sql);
}

// 查询评论总数
exports.commentCount = (wid) => {
    let _sql = `select count(*) as count from comments where wallId="${wid}";`
    return query(_sql);
}

// 某人是否点赞的记录，只能点1次
exports.likeCount = (wid, uid) => {
    let _sql = `select count(*) as count from feedbacks where wallId="${wid}" and userId="${uid}" and type=0;`
    return query(_sql);
}

// 某classId下面、某label的数量
exports.getLabelCounts = (classId, type) => {
    let _sql = `
        SELECT label, COUNT(*) as count 
        FROM walls 
        WHERE classId = ${classId} AND type = ${type}
        GROUP BY label;
    `;
    return query(_sql);
};

// 某label的所有数据
exports.getEntriesByClassTypeLabel = (classId, type, label) => {
    let _sql = `
        SELECT imgurl, name, message
        FROM walls 
        WHERE classId = ${classId} AND type = ${type} AND label = ${label};
    `;
    return query(_sql);
};