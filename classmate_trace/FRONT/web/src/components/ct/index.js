// 管理全部的小属性

import Message from './message/message.js'

// 注册到全局 - 导出一个配置，用于app.use() 安装组件库使用
export default {
    install (app) {
        app.config.globalProperties.$message = Message  // 把之作为全局属性
    }
}