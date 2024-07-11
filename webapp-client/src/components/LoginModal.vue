<template>
<b-modal v-model="showLoginModal" centered title="Login" ok-title="Login"
         ok-variant="outline-success" cancel-variant="outline-danger"
         hide-footer v-on:shown="modalShown" v-on:hidden="modalHidden"
         hide-header-close v-on:hide="modalHiding">
  <b-form @submit="onSubmit" v-bind:class="{ 'lower-opacity': isLoggingIn }">
      <b-form-input
        class="usernameOrEmail-input"
        ref="usernameOrEmailBox"
        type="text"
        v-model="usernameOrEmail"
        required
        placeholder="Username/email"
        v-bind:disabled="isLoggingIn" />

      <b-form-input
        type="password"
        v-model="password"
        required
        placeholder="Password"
        v-bind:disabled="isLoggingIn" />
    <div align="right" class="form-buttons">
      <span class="error-message">{{ errorMessage }}</span>
      <b-button type="button" v-on:click="hide()" variant="outline-danger" class="cancel-button" 
                v-bind:disabled="isLoggingIn">Cancel</b-button>
      <b-button type="submit" variant="outline-success"
                v-bind:disabled="isLoggingIn">
                <b-spinner v-if="isLoggingIn" small class="spinner" type="grow" />
                Login
      </b-button>
    </div>
  </b-form>
</b-modal>
</template>

<script>
export default {
  name: 'LoginModal',
  props: {
    errorMessage: String,
    isLoggingIn: Boolean
  },
  data() {
    return {
      usernameOrEmail: '',
      password: '',
      showLoginModal: false
    }
  },
  methods: {
    show() {
      this.showLoginModal = true
    },
    hide() {
      this.showLoginModal = false;
    },
    modalShown() {
      this.$refs.usernameOrEmailBox.focus();
    },
    modalHidden() {
      this.usernameOrEmail = '';
      this.password = '';
      
      this.$emit('modal-closed');
    },
    modalHiding(evt){
      if (this.isLoggingIn){
        evt.preventDefault();
      }
    },
    onSubmit(evt) {
      evt.preventDefault();

      this.$emit('login-performed', {
        usernameOrEmail: this.usernameOrEmail,
        password: this.password
      });
    },
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.form-buttons {
  margin: 30px 0px 0px 0px;
}

.cancel-button {
  margin: 0px 10px 0px 0px;
}

.error-message {
  margin: 0px 20px 0px 0px;
  color: #fd5454;
}

.username-input {
  margin: 0px 0px 20px 0px;
}

.lower-opacity {
  opacity: 0.9;
}

.spinner {
  vertical-align: center
}
</style>
