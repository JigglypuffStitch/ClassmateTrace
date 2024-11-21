// 引入mock
let Mock = require('mockjs')

// 留言mock - 与后端对应
export const word = Mock.mock({
    "data|19": [{
        // 创建时间
        "moment": new Date(),
        // id（自增）
        "id|+": 1,
        // 游客id
        "userId|+":10,
        // 内容：最多96字，最少24字，cword表示中文
        "message|24-96":"@cword",
        // 标签：原始为数字，对应到标签
        "label|0-8": 0,
        // 用户名（中文名）
        "name":"@cname",
        // 喜欢
        "like|0-120": 0,
        // 评论
        "comment|0-120": 0,
        // 背景颜色（存数字表示，前端去渲染）/图片路径
        "imgurl|0-4": 0,
        // 新留言
        "new|0-20": 0,
        // 是否撤销
        "revoke|0-20": 0,
        // 是否举报
        "report|0-20": 0, 
        // 类型（留言/相册）
        "type": 0, 
    }]
})

// 照片mock
export const photo = Mock.mock({
    "data|25": [{
        // 创建时间
        "moment": new Date(),
        // id（自增）
        "id|+": 1,
        // 游客id
        "userId|+":10,
        // 内容：最多96字，最少24字，cword表示中文
        "message|24-96":"@cword",
        // 标签：原始为数字，对应到标签。此为8个
        "label|0-7": 0,
        // 用户名（中文名）
        "name":"@cname",
        // 喜欢
        "like|0-120": 0,
        // 评论
        "comment|0-120": 0,
        // 背景图片：对应7张静态图片
        "imgurl|0-6": 0,
        // 是否撤销
        "revoke|0-20": 0,
        // 是否举报
        "report|0-20": 0, 
        // 类型：照片
        "type": 1, 
    }]
})

// 评论mock - 与后端对应
export const comment = Mock.mock({
    "data|19": [{
        // 创建时间
        "moment": new Date(),
        // id（自增）
        "id|+": 1,
        // 游客id - 头像对应之，暂时留着
        "userId|+":10,
        // 内容：最多96字，最少24字，cword表示中文
        "message|24-96":"@cword",
        // 用户名（中文名）
        "name":"@cname",
        // 头像颜色，14个
        "imgurl|0-13": 0,
    }]
})