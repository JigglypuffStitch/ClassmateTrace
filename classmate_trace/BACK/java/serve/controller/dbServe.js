const dbModel = require('../lib/db')
const fs = require('fs');

// 新建walls - 对应在routes中通过post触发 - classId加!!!!!!!!
exports.insertWall = async (req, res) => {
    let data = req.body;  // 拿到前端数据
    /*
    // 先不真正插入，通过测试后，前端正确触发后，这里会有信息，再去写数据库
    console.log(data)
    res.send({
        code: 200,
        message: 'aabb',
    }) */

    // 一定要一个个对应起来
    await dbModel.insertWall([data.type, data.message, data.name, data.userId, data.moment, data.label, data.color, data.imgurl, data.classId])
        .then(result => {  // 成功后，前端返回数据
            res.send({
                code: 200,
                message: result, // id可以包括在其中，返回给前端使用这个id
            })
        })
}

// 新建反馈
exports.insertFeedback = async (req, res) => {
    let data = req.body;
    await dbModel.insertFeedback([data.wallId, data.userId, data.type, data.moment])
        .then(result => {
            res.send({
                code: 200,
                message: result,
            })
        })
}

// 新建评论
exports.insertComment = async (req, res) => {
    let data = req.body;
    await dbModel.insertComment([data.wallId, data.userId, data.imgurl, data.moment, data.comment, data.name])
        .then(result => {
            res.send({
                code: 200,
                message: result,
            })
        })
}

// 删除墙
exports.deleteWall = async (req, res) => {
    let data = req.body;
    // 如果地址存在，是图片，则删除图片，因为是url，现在先不管
    if (data.imgurl) {
        // Mkdir.delFiles('data/photo/'+data.imgurl)
        const path = 'data' + data.imgurl;  // 不是data/photo/了！！！！！
        fs.unlink(path, (err) => {
            if (err) {
                console.error('File deletion error:', err);
            } else {
                console.log('File deleted successfully');
            }
        });
    }
    await dbModel.deleteWall(data.id)
        .then((result) => {
            res.send({
                code: 200, 
                message: result,
            })
        })
}

// 删除反馈
exports.deleteFeedback = async (req, res) => {
    let data = req.body;
    await dbModel.deleteFeedback(data.id)
        .then((result) => {
            res.send({
                code: 200,
                message: result,
            })
        })
}

// 删除评论
exports.deleteComment = async (req, res) => {
    let data = req.body;
    await dbModel.deleteComment(data.id)
        .then((result) => {
            res.send({
                code: 200,
                message: result,
            })
        })
}

// 分页查询wall并获取赞、举报、撤销数据 - classId加!!!!!!!!
exports.findWallPage = async (req, res) => {
    let data = req.body;
    await dbModel.findWallPage(data.page, data.pagesize, data.type, data.label, data.classId) // 先找card
        .then(async result => {
            for (let i=0; i<result.length; i++) {
                // 查找对应的数据 - 得到一个较全的result信息
                // 喜欢：每一张card对应的喜欢总数
                result[i].like = await dbModel.feedbackCount(result[i].id, 0);
                // 举报：每一张card对应的举报总数
                result[i].report = await dbModel.feedbackCount(result[i].id, 1);
                // 撤销：每一张card对应的撤销数量
                result[i].revoke = await dbModel.feedbackCount(result[i].id, 2);
                // 是否点赞
                result[i].islike = await dbModel.likeCount(result[i].id, data.userId);
                // 评论
                result[i].comcount = await dbModel.commentCount(result[i].id);
            }
            res.send({
                code: 200,
                message: result,
            })
        })
}

// 倒序分页查询评论
exports.findCommentPage = async (req, res) => {
    let data = req.body
    await dbModel.findCommentPage(data.page, data.pagesize, data.id)
        .then(result => {
            res.send({
                code: 200,
                message: result,
            })
        })
}

// 查询label对应的照片数量
exports.findLabelCounts = async (req, res) => {
    let data = req.body
    await dbModel.getLabelCounts(data.classId, data.type)
        .then(result => {
            res.send({
                code: 200,
                message: result
            })
            console.log(result)
        })

}

// 查询label对应的照片
exports.findLabelItems = async (req, res) => {
    let data = req.body
    await dbModel.getEntriesByClassTypeLabel(data.classId, data.type, data.label)
        .then(result => {
            res.send({
                code: 200,
                message: result
            })
            console.log(result)
        })
}