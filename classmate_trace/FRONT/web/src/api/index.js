import fetch from '@/utils/axios'  // 引入axios文件

// 获取访问者ip
export const signIpApi = () => fetch.post('/signip');

// 新建wall - 对应后端的路由地址
export const insertWallApi = data => fetch.post('/insertwall', data);

// 查询墙
export const findWallPageApi = data => fetch.post('/findwallpage', data);

// 新建反馈
export const insertFeedbackApi = data => fetch.post('/insertfeedback', data);

// 评论
export const insertCommentApi = data => fetch.post('/insertcomment', data);

// 查找评论
export const findCommentPageApi = data => fetch.post('/findcommentpage', data);

// 获取照片
export const profileApi = data => fetch.post('/profile', data);

// 删除card
export const deleteCardApi = data => fetch.post('/deletewall', data);

// 查询label对应的数据数量
export const findLabelCountsApi = data => fetch.post('/findlabelcounts', data);

// 查询label对应的数据
export const findLabelItemsApi = data => fetch.post('/findlabelitems', data);

// ********************************

import axios from 'axios'

// 创建 axios 实例
const request = axios.create({
    baseURL: 'http://172.20.10.2:5140/',
    timeout: 30000,
    headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json'
    },
    withCredentials: false
})
// 请求拦截器
request.interceptors.request.use(
    config => {
        // 可以在这里添加token等认证信息
        return config
    },
    error => {
        return Promise.reject(error)
    }
)
// 响应拦截器
request.interceptors.response.use(
    response => {
        try {
            // 如果响应是空的，返回默认值
            if (!response.data) {
                return { data: [] }
            }
            // 如果响应是字符串，尝试解析
            if (typeof response.data === 'string') {
                return JSON.parse(response.data)
            }
            return response.data
        } catch (error) {
            console.error('Response parse error:', error)
            // 返回默认值
            return { 
                data: [],
                success: false,
                message: error.message
            }
        }
    },
    error => {
        // 更详细的错误处理
        if (error.code === 'ECONNABORTED') {
            console.error('请求超时，请检查后端服务是否正常运行')
            return { data: [] }
        } else if (error.response) {
            console.error('Response error:', error.response.status, error.response.data)
        } else if (error.request) {
            console.error('No response received:', error.request)
        } else {
            console.error('Request error:', error.message)
        }
        return Promise.reject(error)
    }
)

// 添加留言相关的 API
export const commentApi = {
    // 获取留言列表
    getComments(classId) {
        return request({
            url: '/home/comments',
            method: 'get',
            params: {
                class_id: classId
            }
        })
    },
    // 发布留言
    postComment(data) {
        return request({
            url: '/home/comments',
            method: 'post',
            data
        })
    },
    // 点赞留言
    likeComment(data) {
        return request({
            url: '/home/comments',
            method: 'put',
            data
        })
    },
    // 删除留言
    deleteComment(comId) {
        return request({
            url: '/home/comments',
            method: 'delete',
            params: {
                com_id: comId
            }
        })
    }
}

// 添加相册相关的 API
export const albumApi = {
    // 圈出自己
    markSelf(data) {
        return request({
            url: `/album/detail`,
            method: 'post',
            data
        })
    },
    // 取消圈出
    unmarkSelf(params) {
        return request({
            url: '/album/detail',
            method: 'delete',
            params: {
                classmate_id: params.classmate_id,
                picture_id: params.picture_id
            }
        })
    },
    // 读取照片关联同学
    getPictureClassmates(params) {
        return request({
            url: '/album/detail',
            method: 'get',
            params: {
                picture_id: params.picture_id
            }
        })
    },
    // 点赞照片
    likePicture(data) {
        return request({
            url: '/album/detail',
            method: 'put',
            data
        })
    },
    // 读取回忆录
    getMemoirs(classId) {
        return request({
            url: '/album/memoirs',
            method: 'get',
            params: {
                class_id: classId
            }
        })
    },
    // 生成回忆录
    createMemoir(data) {
        return request({
            url: '/album/memoirs',
            method: 'post',
            data
        })
    },
    // 上传照片
    uploadPicture(data) {
        return request({
            url: '/album',
            method: 'post',
            data
        })
    },
    // 读取班级照片
    getClassPictures(classId) {
        return request({
            url: '/album',
            method: 'get',
            params: {
                class_id: classId
            }
        })
    },
    // 管理照片(删除)
    deletePicture(params) {
        return request({
            url: '/album',
            method: 'delete',
            params: {
                classmate_id: params.classmate_id,
                picture_id: params.picture_id
            }
        })
    },
    // 判断tag状态
    isTagged(params) {
        return request({
            url: '/album/detail/conn',
            method: 'get',
            params: {
                user_id: params.userId,
                picture_id: params.photoId,
            }
        })
    },
}

// 添加班级相关的 API
export const classApi = {
    // 创建班级
    createClass(data) {
        return request({
            url: '/home/class/create',
            method: 'post',
            data
        })
    },
    // 切换班级
    switchClass(classId) {
        return request({
            url: '/home/class',
            method: 'get',
            params: {
                class_id: classId
            }
        })
    },
    // 申请加入班级
    applyToClass(data) {
        return request({
            url: '/home/class/apply',
            method: 'post',
            data
        })
    },
    // 审批加入班级
    approveJoinClass(data) {
        return request({
            url: '/home/class/approval',
            method: 'post',
            data
        })
    },
    // 获取审批列表
    getApprovalList(monitorId) {
        return request({
            url: '/home/class/allapply',
            method: 'get',
            params: {
                monitor: monitorId,
            }
        })
    },
    // 获取班级列表
    getClassList(userId) {
        return request({
            url: '/home/class/allclass',
            method: 'get',
            params: {
                user_id: userId,
            }
        })
    },

    // 班长判断
    isMonitor(params) {
        return request ({
            url: '/home/class/monitor',
            method: 'get',
            params: {
                user_id: params.userId,
                class_id: params.classId,
            }
        })

    }
}

// 修改小聚相关的 API
export const gatheringApi = {
    // 获取小聚列表
    getGatheringList(params) {
        return request({
            url: 'home/map/show_gatherings',
            method: 'get',
            params: {
                class_id: params.class_id,
                user_id: params.user_id
            }
        })
    },
    // 创建小聚
    createGathering(params) {
        return request({
            url: '/gathering',
            method: 'post',
            params: {
                class_id: params.class_id,
                datetime: params.datetime,
                address: params.address,
                description: params.description
            },
            headers: {
                user_id: params.user_id
            }
        })
    },
    // 删除小聚
    deleteGathering(params) {
        return request({
            url: '/gathering',
            method: 'delete',
            params: {
                gathering_id: params.gathering_id
            }
        })
    },
    // 加入小聚
    joinGathering(params) {
        return request({
            url: 'gathering/join',
            method: 'put',
            params: {
                gathering_id: params.gathering_id,
            },
            headers: {
                user_id: params.user_id
            }
        })
    },
    // 退出小聚
    exitGathering(params) {
        return request({
            url: '/gathering/exit',
            method: 'put',
            headers: {
                gathering_id: params.gathering_id,
                user_id: params.user_id  // 当前用户ID
            }
        })
    },
    // 修改小聚
    updateGathering(params) {
        return request({
            url: '/gathering/change',
            method: 'put',
            params: {
                datetime: params.datetime,
                address: params.address,
                description: params.description
            },
            headers: {
                user_id: params.user_id,  // 当前用户ID
                gathering_id: params.gathering_id
            }
        })
    },
    // 上传小聚照片
    uploadGatheringPictures(params) {
        return request({
            url: '/gathering/uploadpicture',
            method: 'post',
            data: {
                gathering_id: params.gathering_id,
                class_id: params.class_id,
                address: params.address,
                description: params.description,
                picture_url: params.picture_url
            },
            headers: {
                'user_id': params.user_id
            }
        })
    },
    getGatheringPictures(params) {
        return request({
            url: '/gathering/picture',
            method: 'get',
            params: {
                // gathering_id: String(params.gathering_id),
                gathering_id: params.gathering_id,
                //user_id: params.user_id
            }
        })
    }
}

// 添加地图相关的 API
export const mapApi = {
    // 根据班级同学ID列表查找位置表
    getClassmatesLocations(classId) {
        return request({
            url: '/home/map/show_address',
            method: 'get',
            params: {
                class_id: classId
            }
        })
    },
    // 根据班级同学ID列表查找经纬度位置表
    getClassmatesLLLocations(classId) {
        return request({
            url: '/home/map/show_location',
            method: 'get',
            params: {
                class_id: classId
            }
        })
    },
    // 显示小聚
    getGatherings(params) {
        return request({
            url: '/gathering',
            method: 'get',
            params: {
                class_id: params?.class_id,
                user_id: params?.user_id
            }
        })
    },
    // 修改个人位置
    updateLocation(params) {
        return request({
            url: '/home/map/change',
            method: 'put',
            params: {
                new_add: params.new_add,
                user_id: params.user_id
            }
        })
    },
    // 修改个人经纬度位置
    updateLLLocation(params) {
        return request({
            url: '/home/map/change_location',
            method: 'put',
            params: {
                new_lon: params.new_lon,
                new_lat: params.new_lat,
                user_id: params.user_id
            }
        })
    }
}

// 添加登录注册相关的 API
export const loginregisterApi = {
    // 登录
    loginUser(params) {
        return request({
            url: '/entrance/login',
            method: 'get',
            params: {
                phonenumber: params.phonenumber,
                password: params.password
            }
        })
    },
    // 注册
    registerUser(data) {
        return request({
            url: '/entrance/register',
            method: 'post',
            data
        })
    }
}

// 添加管理相关的 API
export const adminApi = {
    // 获取用户统计量
    getUserStats() {
        return request({
            url: '/admin',
            method: 'get'
        })
    },
    // 更新用户统计量
    updateUserStats() {
        return request({
            url: '/admin',
            method: 'post'
        })
    }
}

// 添加个人空间相关的 API
export const personalApi = {
    // 获取个人留言
    getPersonalComments(userId) {
        return request({
            url: '/personalspace/personalcom',
            method: 'get',
            params: {
                user_id: userId
            }
        })
    },
    // 获取个人信息
    getPersonalInfo(userId) {
        return request({
            url: '/personalspace/info',
            method: 'get',
            params: {
                user_id: userId
            }
        })
    },
    // 获取个人位置
    getPersonalAddress(userId) {
        return request({
            url: '/personalspace/address',
            method: 'get',
            params: {
                user_id: userId
            }
        })
    },
    // 获取个人照片
    getPersonalPictures(userId) {
        console.log("#######################################", userId)
        return request({
            url: '/personalspace/picture',
            method: 'get',
            params: {
                user_id: userId
            }
        })
    },
    // 修改个人信息
    updatePersonalInfo(data) {
        return request({
            url: '/personalspace/info',
            method: 'put',
            data
        })
    },
    // 回复留言
    replyComment(data) {
        return request({
            url: '/personalspace/recom',
            method: 'post',
            data
        })
    },
    // 发布留言
    postComment(data) {
        return request({
            url: '/personalspace/postcom',
            method: 'post',
            data
        })
    },
    // 获取留言详情（二级留言）
    getCommentDetails(comId) {
        return request({
            url: '/personalspace/comdetail',
            method: 'get',
            params: {
                com_id: comId
            }
        })
    }
}

// 添加设置相关的 API
export const settingApi = {
    // 获取用户信息
    getUserInfo(classmate_id) {
        return request({
            url: '/setting',
            method: 'get',
            params: {
                classmate_id: classmate_id
            }
        })
    },
    // 修改用户信息
    updateUserInfo(data) {
        return request({
            url: '/setting',
            method: 'put',
            data
        })
    }
}
// 添加同学列表相关的 API
export const classmateApi = {
    // 获取同学列表
    getClassmateList(classId) {
        return request({
            url: '/classmate',
            method: 'get',
            headers: {
                class_id: classId
            }
        })
    }
} 