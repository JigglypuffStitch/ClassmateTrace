<template>
    <transition name="down">
        <div class="ct-message" v-if="isShow"> <!--控制显示与隐藏-->
            <div class="ct-m-i"> <!--以行的形式封装的-->
                <i class="iconfont" :class="style[type].icon"></i>
                <span class="text">{{ message }}</span>
            </div>
        </div>
    </transition>
</template>

<script>
import { onMounted, ref } from "vue"

export default {
    name: "CTMessage",
    props: { // 从父级带来的变量
        message: {
            type: String,
            default: "",
        },
        type: { // warn 警告 error 错误 success 成功
            type: String,
            default: "warn", 
        },
    },
    setup() {
        // 定义一个对象，包含三种情况的样式，对象key就是类型字符串
        const style = {  // 用于更改class信息
            warning: {
                icon: "icon-warning",
            },
            error: {
                icon: "icon-error"
            },
            success: {
                icon: "icon-success",
            },
        };
        // 定义一个数据控制显示隐藏，默认是隐藏，组件挂载完毕显示
        const isShow = ref(true);  // 控制显示，可以先改成true看一下
        onMounted(() => {
            isShow.value = true;
            setTimeout(() => {
                isShow.value = false;
            }, 3000) // 3s
        });
        return { style, isShow };
    },
}
</script>

<style lang="less" scoped>
// 动画效果
.down {
    &-enter {
        &-from {
            transform: translateY(-30px);  // 从往上的30px
            opacity: 0;
        }
        &-active {
            transition: all 0.3s;
        }
        &-to {
            transform: translateY(0px);
            opacity: 1;
        }
    }
    &-leave {
        &-from {
            transform: translateY(0px);
            opacity: 1;
        }
        &-active {
            transition: all 0.3s;
        }
        &-to {
            transform: translateY(-30px);
            opacity: 0;
        }
    }
}
.ct-message{
    width: 100%;
    height: 40px;
    position: fixed;
    z-index: 9999;
    top: 20px;
    line-height: 40px;
    display: flex;  // 内容定在中间
    justify-content: center;
    .ct-m-i {
        padding: 0 20px;
        border-radius: 8px;
        color: @gray-0;
        background: @gray-4;
        box-shadow: 0 4px 16px rgba(0,0,0,0.1);
    }
    i { // 小图标
        margin-right: 6px;
        vertical-align: middle;  // 上下居中
    }
    .text {
        vertical-align: middle;
    }
}

// icon颜色
.icon-success {
    color: @primary-color;
}
.icon-warning {
    color: @warning-color;
}
.icon-error {
    color: @error-color;
}

</style>