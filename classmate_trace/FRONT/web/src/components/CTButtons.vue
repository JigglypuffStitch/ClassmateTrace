<template>
    <button :class="[size,nom]" class="ct-buttons"><slot></slot></button> <!--通过slot让按钮可自定义风格-->
</template>

<script>
export default {
    props: {
        size: {
            default: 'base', // 默认为中
        },
        nom: {
            default: 'primary', // 默认为主
        }
    }
}
</script>

<style lang="less" scoped>
.ct-buttons {
    text-align: center; // 居中，可能需要配合line-height来保证高度正确
    font-size: 14px;;
    border: none; // 不要边框
    cursor: pointer; // 指针样式
}
// size
.max {
    min-width: 100px;
    height: 48px;
    border-radius: 24px;
    padding: 0 24px; // 留左右间距
}
.base {
    min-width: 80px;
    height: 36px;
    border-radius: 24px;
    padding: 0 20px; // 留左右间距
}
.small {
    min-width: 72px;
    height: 32px;
    border-radius: 24px;
    padding: 0 20px;
}
// nom
.primary {
    background: @gray-0;
    color: @gray-4;
}
.secondary {
    background: none;  // 不要背景色
    color: @gray-0;
    border: 1px solid @gray-0; // 边栏
}
.c_primary { // 带颜色
    background: @primary-color;
    color: @gray-4;
    font-weight: 400;
}
.c_secondary { // 带颜色
    background: @gray-4;
    color: @gray-0;
}
.notallowed {
    opacity: 0.6;
    cursor: not-allowed;
}
</style>