import { createApp } from 'vue'
import App from './App.vue'
import router from './router/index'
import store from './store/index'

import ElementPlus from 'element-plus';
import 'element-plus/dist/index.css';
import zhCn from 'element-plus/es/locale/lang/zh-cn'

// import axios from 'axios' // axios现在已经不需要在这里引用了
// import VueAxios from 'vue-axios'

// 自建组件
import CT from '@/components/ct/index'

const app = createApp(App)
app.use(ElementPlus, {
    locale: zhCn,
});
app.use(router);
app.use(store);
// app.use(VueAxios, axios);
app.use(CT); 


app.mount('#app') 