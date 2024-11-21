<template>
    <transition name="view">
        <div class="ct-viewer" v-if="isView">
            <div class="bg"></div>
            <div class="viewer-photo">
                <img :src="photos[currentNumber]" class="photo-img"/> <!--先静态-->
            </div>
            <div class="switch sw-left" @click="changeNumber(0)" v-show="currentNumber>0">
                <span class="iconfont icon-xiangzuo"></span>
            </div>
            <div class="switch sw-right" @click="changeNumber(1)" v-show="currentNumber<photos.length-1">
                <span class="iconfont icon-xiangyou"></span>
            </div> 
        </div>
    </transition>
</template>

<script>
import { baseUrl } from '@/utils/env'
export default {
    data() {
        return {
            baseUrl,
        }
    },
    props: {
        photos: {  // 上级传进来是数组
            default: [],
        }, 
        currentNumber: {  // 当前照片在数组中的索引，为了统一修改左右内容，让父级来修改之，并告诉本级
            type: Number,
            default: 0,
        },
        isView: {
            default: false, // 类似isModal
        }
    },
    /*
    computed: {
        number(){
            return this.currentNumber;
        }
    }, */
    methods: {
        changeNumber(e) { // 0向左，1向右
            this.$emit('viewSwitch', e);
        }
    }
}
</script>

<style lang="less" scoped>
.view {
    &-enter { // 入场动画，这里只变化透明度
        &-from { 
            opacity: 0;
        }
        &-active { 
            transition: all 0.2s ease-out;
        }
        &-to {
            opacity: 1;
        }
    }
    &-leave { // 出场动画
        &-from {
            opacity: 1;
        }
        &-active {
            transition: all 0.2s ease-in;  // 慢速开始
        }
        &-to {
            opacity: 0;
        }
    }
}
.ct-viewer {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    .bg {
        position: absolute;
        top: 0;
        left: 0;
        background: rgba(255,255,255,0.9);
        backdrop-filter: blur(20px);
        height: 100%;
        width: 100%;
    }
    .viewer-photo {
        position: absolute;
        padding: 82px 454px 30px 96px;
        box-sizing: border-box;
        width: 100%;
        height: 100%;
        overflow-y: auto;
        display: flex;
        justify-content: center; // 左右居中
        img {
            max-width: 100%;
        }
    }
    .switch {
        width: 56px;
        height: 56px;
        border-radius: 50%;
        background: @gray-2;
        color: @gray-4;
        position: absolute; // 绝对定位
        top:30px;
        bottom: 0;
        margin: auto;
        display: flex;
        align-items: center; // 上下居中
        justify-content: center; // 左右居中
        opacity: 0.5;
        transition: @tr;
        cursor: pointer;
        .iconfont {
            font-size: 24px;
        }
        &:hover {
            opacity: 1;
        }
    }
    .sw-left {
        left: 20px;
    }
    .sw-right {
        right: 380px;
    }
}

</style>