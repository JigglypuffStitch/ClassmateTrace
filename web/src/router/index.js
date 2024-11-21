import { createRouter, createWebHashHistory } from "vue-router";
import store from "../store"; // 引入 Vuex store

const routes = [
    {
        path: '/',
        redirect: '/login', // 默认跳转到登录页面
    },
    {
        path: '/login',
        name: 'login',
        component: () => import('../views/CTLoginRegister.vue') // 登录页面
    },
    {
        path: '/admin',
        name: 'admin',
        component: () => import('../views/AdminInterface.vue') // 登录页面
    },
    {
        path:'/',
        // redirect:'/map',  // 先把路由写死
        name:'index',
        component: () => import('../views/CTIndex.vue'),
        children:[
            {
                path:'wall',
                component: () => import('../views/MessageWall.vue')
            },
            {
                path: 'map',
                component: () => import('../views/MapView.vue')
            },
            {
                path:'gathering',
                component: () => import('../views/MeetingRoom.vue')
            },
            {
                path: 'setting',
                component: () => import('../views/UserSetting.vue')
            },
            {
                path: 'personalSpace',
                component: () => import('../views/PersonalSpace.vue')
            }
        ]
    }
]

const router = createRouter({
    history: createWebHashHistory(),
    routes,
})

// 路由导航守卫
router.beforeEach((to, from, next) => {
    if (to.name !== 'login' && !store.state.isLoggedIn) {
        console.log('未登录')
        // 如果未登录且尝试访问非登录页，则跳转到登录页面
        next({ name: 'login' });
    } else {
        // 否则放行
        next();
    }
});

export default router;