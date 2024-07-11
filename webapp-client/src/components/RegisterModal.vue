<template>
<b-modal v-model="showRegisterModal" centered title="Register" ok-title="Register"
         ok-variant="outline-success" cancel-variant="outline-danger"
         hide-footer v-on:shown="modalShown" v-on:hidden="modalHidden"
         hide-header-close v-on:hide="modalHiding">
  <div align="center" v-if="successfulRegister">
    <span id="successfulRegisterIcon">
      <fa-icon icon="check-circle" size="7x"></fa-icon>
    </span>
    <div id="successfulRegisterText">
      <h4>Success!</h4>
      <h5>You can now login using your username or email.</h5>
    </div>
    <div align="right" class="form-buttons">
      <b-button type="button" v-on:click="hide()" variant="outline-success">Close</b-button>
    </div>
  </div>
  <b-form v-else @submit="onSubmit" v-bind:class="{ 'lower-opacity': isRegistering }">
    <b-form-group
        id="usernameInputGroup"
        label="Username: "
        label-for="usernameInput"
        :invalid-feedback="usernameErrors[0]"
        :valid-feedback="usernameChecked && isUsernameAvailable ? 'Your username is available!' : null"
        :state="this.$v.username.$dirty ? usernameErrors.length == 0 : null"
    >
      <b-form-input
        id="usernameInput"
        ref="usernameBox"
        type="text"
        name="username"
        v-model="username"
        placeholder="Enter username"
        :state="this.$v.username.$dirty ? usernameErrors.length == 0 : null"
        v-on:input="this.$v.username.$touch"
        :disabled="isRegistering" />
    </b-form-group>

    <b-form-group
        id="passwordInputGroup"
        label="Password: "
        label-for="passwordInput"
        :invalid-feedback="passwordErrors[0]"
        :state="this.$v.password.$dirty ? passwordErrors.length == 0 : null"
    >
      <b-form-input
        id="passwordInput"
        type="password"
        name="password"
        v-model="password"
        placeholder="Enter password"
        :state="this.$v.password.$dirty ? passwordErrors.length == 0 : null"
        v-on:input="this.$v.password.$touch"
        :disabled="isRegistering" />
    </b-form-group>
    <b-form-group
        id="confirmPasswordInputGroup"
        label="Confirm password: "
        label-for="confirmPasswordInput"
        :invalid-feedback="confirmPasswordErrors[0]"
        :state="this.$v.confirmPassword.$dirty ? confirmPasswordErrors.length == 0 : null"
    >
      <b-form-input
        id="confirmPasswordInput"
        type="password"
        name="confirmPassword"
        v-model="confirmPassword"
        placeholder="Re-enter password"
        :state="this.$v.confirmPassword.$dirty ? confirmPasswordErrors.length == 0 : null"
        v-on:input="this.$v.confirmPassword.$touch"
        v-bind:disabled="!this.$v.password.$dirty || this.$v.password.$error || isRegistering" />
    </b-form-group>

    <b-form-group
        id="emailInputGroup"
        label="Email: "
        label-for="emailInput"
        :invalid-feedback="emailErrors[0]"
        :valid-feedback="emailChecked && isEmailAvailable ? 'That email is not yet registered!' : null"
        :state="this.$v.email.$dirty ? emailErrors.length == 0 : null"
    >
      <b-form-input
        id="emailInput"
        type="text"
        name="email"
        v-model="email"
        placeholder="Enter email"
        :state="this.$v.email.$dirty ? emailErrors.length == 0 : null"
        v-on:input="this.$v.email.$touch"
        :disabled="isRegistering" />
    </b-form-group>

    <div align="right" class="form-buttons">
      <span class="error-message">{{ errorMessage }}</span>
      <b-button type="button" v-on:click="hide()" variant="outline-danger" class="cancel-button" 
                v-bind:disabled="isRegistering">Cancel</b-button>
      <b-button type="submit" variant="outline-success"
                v-bind:disabled="isRegistering || !canSubmit">
                <b-spinner v-if="isRegistering" small class="spinner" type="grow" />
                Register
      </b-button>
    </div>
  </b-form>
</b-modal>
</template>

<script>
import { required, minLength, maxLength, sameAs, email } from 'vuelidate/lib/validators';
import { debounce } from 'lodash';
import AvailabilityApi from '../services/api/Availability.js';

const alphaNumHyphenUnderscore = value => {
  if (typeof value === 'undefined' || value == null || value === '') {
    return true;
  }

  return /^[A-Za-z0-9_-]*$/.test(value);
}

export default {
  name: 'RegisterModal',
  props: {
    isRegistering: Boolean,
    successfulRegister: Boolean,
    errorMessage: String
  },
  data() {
    return {
      username: '',
      password: '',
      confirmPassword: '',
      email: '',
      fullName: '',

      showRegisterModal: false,

      usernameChecked: false,
      isUsernameAvailable: false,
      
      emailChecked: false,
      isEmailAvailable: false,
    }
  },
  watch: {
    username: function () {
      this.usernameChecked = false;
      this.getIsUsernameAvailable();
    },
    email: function() {
      this.emailChecked = false;
      this.getIsEmailAvailable();
    }
  },
  mounted: function() {
    
  },
  methods: {
    getIsUsernameAvailable: debounce(async function() {
      if (this.username === '' || this.$v.username.$error) {
        return;
      }

      try {
        var data = await AvailabilityApi.getUsernameAvailability(this.username); 
        this.isUsernameAvailable = data.isAvailable;
        this.usernameChecked = true;
      }
      catch(error) {
        if (!error.response) {
          this.$emit('server-down');
        }
      }
    }, 750),
    getIsEmailAvailable: debounce(async function() {
      if (this.email === '' || this.$v.email.$error) {
        return;
      }
      
      try {
        var data = await AvailabilityApi.getEmailAvailability(this.email);
        this.isEmailAvailable = data.isAvailable;
        this.emailChecked = true;
      }
      catch(error) {
        if (!error.response) {
          this.$emit('server-down');
        }
      }
    }, 750),
    show() {
      this.showRegisterModal = true
    },
    hide() {
      this.showRegisterModal = false;
    },
    modalShown() {
      !this.successfulRegister && this.$refs.usernameBox.focus();
    },
    modalHidden() {
      this.username = '';
      this.password = '';
      this.confirmPassword = '';
      this.email = '';

      this.$v.$reset();
      
      this.$emit('modal-closed');
    },
    modalHiding(evt) {
      if (this.isRegistering){
        evt.preventDefault();
      }
    },
    onSubmit(evt) {
      evt.preventDefault();

      this.$emit('register-performed', {
        username: this.username,
        password: this.password,
        email: this.email,
        fullName: this.fullName
      });
    },
  },
  validations: {
    username: {
      required,
      alphaNumHyphenUnderscore,
      maxLength: maxLength(20)
    },
    password: {
      required,
      minLength: minLength(8)
    },
    confirmPassword: {
      sameAsPassword: sameAs('password')
    },
    email: {
      required,
      email
    }
  },
  computed: {
    usernameErrors() {
      var errors = [];
      if (!this.$v.username.$dirty){
        return errors;
      }

      !this.$v.username.required && errors.push('Username is required.');
      !this.$v.username.maxLength && errors.push('Username must not be more than 20 characters in length.');
      !this.$v.username.alphaNumHyphenUnderscore && errors.push('Username can only include alphabets, numbers, hyphens, and underscores.');
      this.usernameChecked && !this.isUsernameAvailable && errors.push('Sorry, that username is taken.');
      return errors;
    },
    passwordErrors() {
      var errors = [];
      if (!this.$v.password.$dirty) {
        return errors;
      }

      !this.$v.password.required && errors.push('Password is required.');
      !this.$v.password.minLength && errors.push('Your password must be at least 8 characters long.');
      return errors;
    },
    confirmPasswordErrors() {
      var errors = [];
      if (!this.$v.confirmPassword.$dirty) {
        return errors;
      }

      !this.$v.confirmPassword.sameAsPassword && errors.push('Passwords do not match.');
      return errors;
    },
    emailErrors() {
      var errors = [];
      if (!this.$v.email.$dirty) {
        return errors;
      }

      !this.$v.email.required && errors.push('Email is required.');
      !this.$v.email.email && errors.push('Please enter a valid email address.');
      this.emailChecked && !this.isEmailAvailable && errors.push('Sorry, that email is already registered.');
      return errors;
    },
    canSubmit() {
      return this.usernameChecked && this.emailChecked
          && this.isUsernameAvailable && this.isEmailAvailable
          && !this.$v.$error && this.$v.$dirty;
    }
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

#successfulRegisterIcon {
  color: #43ff63
}

#successfulRegisterText {
  margin: 20px 0px 0px 0px;
}
</style>
