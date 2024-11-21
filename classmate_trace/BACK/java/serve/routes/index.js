const controller = require('../controller/dbServe') // 引用dbServe.js，而routes又被外层index.js引用

module.exports = function(app) {
    // 测试
    app.get('/test', (req,res)=> {
        res.type('html');
        res.render('test');
    })
    // 新建wall数据
    app.post('/insertwall', (req, res) => {
        controller.insertWall(req, res)
    })
    // 新建反馈
    app.post('/insertfeedback', (req, res) => {
        controller.insertFeedback(req, res)
    })
    // 新建评论
    app.post('/insertcomment', (req, res) => {
        controller.insertComment(req, res)
    })
    // 删除墙
    app.post('/deletewall', (req, res) => {
        controller.deleteWall(req, res)
    })
    // 删除反馈
    app.post('/deletefeedback', (req, res) => {
        controller.deleteFeedback(req, res)
    })
    // 删除评论
    app.post('/deletecomment', (req, res) => {
        controller.deleteComment(req, res)
    })
    // 分页查询
    app.post('/findwallpage', (req, res) => {
        controller.findWallPage(req, res)
    })
    // 评论查询
    app.post('/findcommentpage', (req, res) => {
        controller.findCommentPage(req, res)
    })
    // 用户进入进行ip登记
    app.post('/signip', (req, res) => {
        var ip = req.ip;
        res.send({
            code: 200,
            ip: ip,  // 本机访问都是localhost，就是0
        })
    })
    // 找到label条目数
    app.post('/findlabelcounts', (req, res) => {
        controller.findLabelCounts(req, res)
    })
    // 找到label条目
    app.post('/findlabelitems', (req, res) => {
        controller.findLabelItems(req, res)
    })
}