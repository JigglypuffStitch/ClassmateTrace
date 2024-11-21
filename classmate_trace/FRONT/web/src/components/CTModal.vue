<template>
    <transition name="modal">
        <div class="ct-modal" v-if="isModal"> <!--父级改变之-->
            <div class="ct-modal-head"> <!--统一的head-->
                <p class="modal-name">{{title}}</p> <!--当变量-->
                <span class="iconfont icon-guanbi" @click="closeModal"></span> <!--按钮触发-->
            </div>
            <div class="ct-modal-main"> <!--包裹-->
                <slot></slot>
                <!--<div class="slot"></div>-->
            </div>
        </div>
    </transition>
</template>

<script setup>
import { defineProps, defineEmits } from 'vue';

const props = defineProps({
    title:{
        default: '标题', // 可于上层中指定值
    }, 
    isModal: {
        default: false, // 为真展开，为假关闭
    }
});

const emit = defineEmits(['close']); // 关闭时用之来通讯

function closeModal() {
    emit('close'); // 发消息到上层
}

props;

</script>

<style lang="less" scoped>
.modal {
    &-enter { // 入场动画
        &-from {  // 来自
            transform: translateX(360px); // x轴右移360px
        }
        &-active { // 过程动画
            transition: all 0.2s ease-out; // 缓动，慢速结束
        }
        &-to {  // 到哪去
            transform: translateX(0px);
        }
    }
    &-leave { // 出场动画
        &-from {
            transform: translateX(0px); 
        }
        &-active {
            transition: all 0.2s ease-in;  // 慢速开始
        }
        &-to {
            transform: translateX(360px);
        }
    }
}
.ct-modal {
    width: 360px;
    height: 100%;
    position: fixed; // 绝对定位
    right: 0;
    top: 52px;
    z-index: 1000;
    background: rgba(255,255,255,0.80);
    box-shadow: 0px 0px 20px 0px rgba(0,0,0,0.08);
    backdrop-filter: blur(20px); // 毛玻璃
    .ct-modal-head {
        display: flex;
        justify-content: space-between; // 左右布局
        align-items: center;
        padding: @padding-20;  // 一圈边距，放里面，让滚动条在外面
        box-sizing: border-box;

        .modal-name {
            font-size: 16px;
            color: @gray-0;
            font-weight: 600;
        }
        .icon-guanbi {
            color: @gray-1;
            cursor: pointer;
            padding-left: 8px;  // 让cursor范围更大
        }
    }
    .ct-modal-main {
        // border: 1px solid #eee;
        height: 100%; // 用100%的高度
        overflow-y: auto;
        padding-bottom: 130px;
        box-sizing: border-box;
        overflow-x: auto;
        margin: 2px;
    }
    .slot {
        height: 1200px;
        background: #eee;
    }
    .ct-modal-main::-webkit-scrollbar {
        // 滚动条整体样式，高宽分别对应横竖滚动条的尺寸
        width: 4px;
        height: 4px;
    }
    .ct-modal-main::-webkit-scrollbar-thumb {
        // 滚动条里面的小方块
        border-radius: 4px;  // 圆角
        background: rgba(0,0,0,0.2);
    }
    .ct-modal-main::-webkit-scrollbar-track {
        // 滚动条里面的轨道
        border-radius: 4px;
        background: rgba(0,0,0,0);
    }

}

</style>