<template>
  <div class="modal-overlay">
    <div class="modal">
      <h3>编辑签名</h3>
      <textarea v-model="newSignature" :placeholder="signature"></textarea>
      <div class="modal-actions">
        <button class="save-button" @click="save">保存</button>
        <button class="cancel-button" @click="cancel">取消</button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  props: ['signature'],
  data() {
    return {
      newSignature: this.signature // Pre-fill the text area with the current signature
    };
  },
  methods: {
    save() {
      this.$emit('save', this.newSignature);
    },
    cancel() {
      this.$emit('close');
    }
  }
};
</script>

<style scoped>
/* Styles for modal */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.7);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal {
  background: white;
  padding: 30px;
  border-radius: 10px;
  width: 400px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
  animation: fadeIn 0.3s ease-out;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: scale(0.9);
  }
  to {
    opacity: 1;
    transform: scale(1);
  }
}

h3 {
  margin-bottom: 20px;
  font-size: 1.5em;
  color: #333;
  text-align: center;
}

textarea {
  width: 100%;
  height: 120px;
  margin-bottom: 20px;
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 5px;
  font-size: 1em;
  resize: none;
  box-shadow: inset 0 1px 3px rgba(0, 0, 0, 0.1);
}

.modal-actions {
  display: flex;
  justify-content: space-between;
}

button {
  padding: 10px 20px;
  border: none;
  border-radius: 5px;
  font-size: 1em;
  cursor: pointer;
  transition: background 0.3s ease;
}

.save-button {
  background: #4CAF50;
  color: white;
}

.save-button:hover {
  background: #45a049;
}

.cancel-button {
  background: #f44336;
  color: white;
}

.cancel-button:hover {
  background: #e53935;
}
</style>