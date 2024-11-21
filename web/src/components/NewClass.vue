<template>
    <transition name="newclass">
        <div class="modal-overlay" @click.self="close">
        <div class="modal">
            <div class="modal-header">
                <span>新建班级</span>
                <span class="iconfont icon-guanbi" @click="close"></span> 
            </div>

            <div class="modal-body">
                <input
                    type="text"
                    v-model="className"
                    placeholder="请输入班级名称"
                    class="input-field"
                />
            </div>

            <div class="modal-footer">
                <button class="confirm-button" @click="clickConfirm">新建</button>
            </div>
        </div>
        </div>
    </transition>

</template>

<script>
import { classApi } from '@/api/index'
export default {
    data() {
        return {
            className: "", // 用于存储用户输入的班级名称
            userId: this.$store.state.user.id,
        };
    },
    methods: {
        close() {
            this.$emit("closeCreate");
            this.className = '';
        },
        async clickConfirm() {
            let data = {
                className: this.className,
                classTime: new Date(),
                monitor: this.userId,
            }
            console.log(data);
            try {
                const response = await classApi.createClass(data);
                if (response) {
                    this.$message({ type: "success", message: response.name+"新建成功！" });
                    // this.$emit("newClassHere")
                }
                else {
                    this.$message({ type: "error", message: "新建失败。" });
                }
            }
            catch (e) {
                console.error(e)
                this.$message({ type: "error", message: "新建失败。" });
            }
            this.close();
            this.className = '';
        },
    },
};
</script>
  
<style lang="less" scoped>
.newclass {
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
  .modal-overlay {
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background-color: rgba(0, 0, 0, 0.5);
    display: flex;
    align-items: center;
    justify-content: center;
  }
  
  .modal {
    background-color: white;
    width: 300px;
    border-radius: 8px;
    overflow: hidden;
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.468);
    justify-content: center;
  }
  
  .modal-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    height: 56px;
    padding: 16px 24px;
    font-size: 18px;
    font-weight: bold;
    border-bottom: 1px solid #ddd;
    box-sizing: border-box;
  }

  
  .icon-guanbi {
    background: none;
    border: none;
    color: @gray-1;
    cursor: pointer;
  }
  
  .modal-body {
    padding: 30px 30px 16px;
  }
  
  .input-field {
    width: 100%;
    padding: 8px;
    font-size: 16px;
    box-sizing: border-box;
  }
  
  .modal-footer {
    display: flex;
    justify-content: center;
    padding: 16px 16px 25px;
  }
  
  .confirm-button {
    padding: 8px 16px;
    font-size: 16px;
    background-color: #007bff;
    color: white;
    border: none;
    border-radius: 6px;
    cursor: pointer;
    transition: @tr;
  }
  
  .confirm-button:hover {
    background-color: #0056b3;
  }
</style>