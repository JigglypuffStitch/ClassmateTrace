const express = require('express') // 引入express框架
const path = require('path') // 引入node中的路径

// 解析html
var ejs = require('ejs');  // 引入ejs插件
var config = require('./config/default')  // 把config中的最基本参数拿到

const app = express() // 创建express的app

// 获取静态路径
// app.use(express.static(__dirname+'/dist'))  // use静态路径，可从前端访问此路径 => 前端build会生成，可丢给后端一级目录，让后端访问
app.use(express.static(__dirname+'/views'))  // 测试
app.use(express.static(__dirname+'/data'))  // data存放后端上传的图片，在浏览器中打印地址，便可以显示

// 设置允许跨域访问该服务 - 让浏览器可以访问云服务器
app.all('*', function(req, res, next) {
    // 允许访问ip *为所有
    res.header("Access-Control-Allow-Origin", "*");
    res.header("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
    res.header("Access-Control-Allow-Credentials", true);
    res.header("Access-Control-Allow-Methods", "PUT,POST,GET,DELETE,OPTIONS");
    res.header("X-Powered-By", '3,2,1')
    // 为了方便返回json
    res.header("Content-Type", "application/json;charset=utf-8");
    if (req.method == 'OPTIONS') {
        // 让options请求快速返回
        res.sendStatus(200);
    }
    else {
        next();
    }
});

// 加入html视图
app.engine('html', ejs.__express);  // html给ejs解析，让之显示正常页面，而非html源码
app.set('view engine', 'html');

// 解析前端json数据项（可以let data = req.body;来获取前端传来的数据项）
app.use(express.json())
app.use(express.urlencoded({ extended: true }))

// 引入路由，方便接受前端的post等请求
require('./routes/index')(app);
require('./routes/files')(app);

// 监听端口号，./config/default中的
app.listen(config.port, () => {
    console.log(`我启动了端口${config.port}`)
})