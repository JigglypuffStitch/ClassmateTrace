// 引入插件
var multer = require('multer');

// 生成随机数
function random(min, max) {
    return Math.floor(Math.random() * (max-min))+min;
}

const storage = multer.diskStorage({
    // 保存路径
    destination: function (req, file, cb) {
        cb(null, './data/photo')  // 保存到photo下，注意只有一个点
    },
    // 保存在destination中的文件名
    filename: function (req, file, cb) {
        let type = file.originalname.replace(/.+\./, ".")  // 正则匹配后缀名
        cb(null, Date.now()+ random(1,100) + type)  // 随机数 
    }
})

const upload = multer({ storage: storage })

module.exports = function(app) {
    app.post('/profile', upload.single('file'), function (req, res, next)  { // 'file'要对应前端来的数据项
        let name = req.file.filename // req.file是'avatar'对应的文件信息
        console.log(name)
        let imgurl = '/photo/'+name;
        res.send(imgurl) // 前端反馈
    })
}

