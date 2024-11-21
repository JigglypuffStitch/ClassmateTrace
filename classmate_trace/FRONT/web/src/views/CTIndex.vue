<template>
    <div class="ct-index">
        <top-bar> </top-bar>
        <video src="@/assets/images/bg1.mp4" autoplay="autoplay" muted="muted" loop="loop" class="bg-video"> </video>
        <router-view></router-view>
        <foot-bar-vue></foot-bar-vue>
    </div>
</template>

<script>
import topBar from '@/components/TopBar.vue'
import footBarVue from '@/components/FootBar.vue'
// import { signIpApi } from '@/api/index' // 引入api

export default {
    data() {
        return {
            aaa: '',
        }
    },

    components: {
        // 组件
        topBar,
        footBarVue,
    },
    computed: {
        // 变量
    },
    created() {
        // this.getUser();
    },
    methods: {
        /*
        getUser(){
            signIpApi().then((res) => {
                // console.log(res)
                let user = {
                    id: res.ip,
                }
                this.$store.commit('getUser', user) // 对应store/index中的方法，页面中可以直接用this访问的，因为在app.vue中已经引用了
            })
        }*/
    }
}
  
</script>

<style lang="less" scoped>
.ct-index {
    .bg-video {
        height:"840";
        position: fixed; // 不随着页面滚动
        top:0;
        left:0;
        z-index:-1;
    }
}

</style> 