import { baseUrl } from './env'

import axios from 'axios';

const service = axios.create({
    baseURL: baseUrl,
    timeout: 10000 // 超时阈值，本地5s
})

service.interceptors.request.use(  // 添加请求拦截器
    // 在发送请求前做些什么
    config => {
        return config;
    },
    // 对请求错误做些什么
    error => {
        console.log(error);
        return Promise.reject();
    }
);

service.interceptors.response.use(  // 添加响应拦截器
    // 对响应数据做点什么
    response => {
        if (response.status === 200) {
            return response.data;
        }
        else {
            Promise.reject();
        }
    },
    // 对响应错误做点什么
    error => {
        console.log(error);
        return Promise.reject();
    }
);

export default service;