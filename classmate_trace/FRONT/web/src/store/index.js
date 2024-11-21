import { createStore } from 'vuex'

// 创建一个新的 store 实例
const store = createStore({
  state () {
    return {
      count: 0,
      user: 0, // 用户身份变量
      classId: 0,
      isLoggedIn: false, // 登录状态
    }
  },
  mutations: {
    increment (state) {
      state.count++
    },
    getUser(state, n) {
      state.user = n; // 用于覆盖身份信息的函数
    },
    getClass(state, n) {
      state.classId = n;
    },
    setLoginStatus(state, status) {
      state.isLoggedIn = status; // 更新登录状态
    },
    logout(state) {
      state.user = ''; // 清空用户信息
      state.isLoggedIn = false; // 设置为未登录
    },
  }
})

export default store; // 引出
