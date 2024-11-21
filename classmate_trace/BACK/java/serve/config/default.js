/*
const config = {
    port: 3000, // 后端默认启动端口号，我的电脑上是3306
    database: {
        HOST: 'localhost',
        USER: 'root',  // 用户
        PASSWORD: '212798SF',  // 本地数据库的密码
        WALL: 'WALLTS',
    }
}

module.exports = config
*/

const config = {
    port: 3000, // 后端默认启动端口号，我的电脑上是3306
    database: {
        HOST: '172.20.10.2',
        USER: 'SF',  // 用户
        PASSWORD: '123456',  // 本地数据库的密码
        WALL: 'classmate_trace',
    }
}

module.exports = config