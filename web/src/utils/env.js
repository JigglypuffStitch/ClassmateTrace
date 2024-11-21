// 配置编译环境和线上环境之间的切换

// baseUrl: 域名地址
// routerMode: 路由模式
// baseImgPath: 图片存放地址

let baseUrl = '';
let routerMode = 'hash';
let baseImgPath;

if (process.env.NODE_ENV == 'development') { // 测试（本地）环境的url
    baseUrl = 'http://localhost:3000';
    baseImgPath = 'http://localhost:3000';
}
else {
    // 有服务器时再修改
    baseUrl = 'http://localhost:3000';
    baseImgPath = 'http://localhost:3000';
}

export {
    baseUrl, 
    routerMode,
    baseImgPath
}