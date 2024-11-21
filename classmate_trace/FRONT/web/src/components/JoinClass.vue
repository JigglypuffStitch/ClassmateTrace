<template>
    <transition name="join">
        <div class="modal-overlay" @click.self="close">
        <div class="modal">
            <div class="modal-header">
                <span>加入班级</span>
                <span class="iconfont icon-guanbi" @click="close"></span> 
            </div>

            <div class="modal-body">
                <input
                    type="text"
                    v-model="classCode"
                    placeholder="请输入班级号码"
                    class="input-field"
                />
            </div>

            <div class="modal-footer">
                <button class="confirm-button" @click="clickConfirm">申请</button>
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
            classCode: "", // 用于存储用户输入的班级号码
            userId: this.$store.state.user.id,
        };
    },
    methods: {
        close() {
            this.$emit("closeJoin");
            this.classCode = '';
        },
        async clickConfirm() {
            let data = {
                classId: this.classCode,
                userId: this.userId,
            }
            // console.log("Class Code:", this.classCode);
            try {
                const response = await classApi.applyToClass(data);
                if (response.response == "Application to join class successful.") {
                    this.$message({ type: "success", message: "申请成功！" });
                }
                else {
                    this.$message({ type: "error", message: "申请失败。" });
                }
            }
            catch (e) {
                console.error(e)
                this.$message({ type: "error", message: "申请失败。" });
            }
            this.close();
            this.classCode = '';
        },
    },
};
</script>
  
<style lang="less" scoped>
.join {
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