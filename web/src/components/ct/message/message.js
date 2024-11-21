import { createVNode, render } from 'vue'
import ctMessage from './CTMessage.vue'

const divVNode = createVNode('div', { class: 'xtx-message-container' })  // 创建div
render(divVNode, document.body)  // body中创建

const div = divVNode.el

const CTMessage = ({ message, type }) => {
    // 动态创建虚拟DOM
    const comVNode = createVNode(ctMessage, { message, type })  // 插入内容
    // 渲染到body中，插入到上面的div中
    render(comVNode, div)
    // 6s后自动卸载
    setTimeout(() => {
        render(null, div)
    }, 6000)
}

export default CTMessage