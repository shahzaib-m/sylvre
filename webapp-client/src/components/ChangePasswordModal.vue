<template>
<b-modal v-model="showModal" title="Change your password" hide-header-close hide-footer
				 v-on:shown="modalShown" v-on:hidden="modalHidden" v-on:hide="modalHiding">
	<b-form @submit="onSubmit" v-bind:class="{ 'lower-opacity': isChangingPassword }">
		<b-form-group
			id="passwordInputGroup"
			label="Current password: "
			label-for="passwordInput"
			:invalid-feedback="currentPasswordErrors[0]"
			:state="this.$v.currentPassword.$dirty ? currentPassword.length == 0 : null"
		>
		<b-form-input
			id="currentPasswordInput"
			ref="currentPasswordBox"
			type="password"
			name="currentPassword"
			v-model="currentPassword"
			placeholder="Enter current password"
			:state="this.$v.currentPassword.$dirty ? currentPasswordErrors.length == 0 : null"
			v-on:input="this.$v.currentPassword.$touch"
			:disabled="isChangingPassword" />
		</b-form-group>

		<b-form-group
        id="passwordInputGroup"
        label="New password: "
        label-for="passwordInput"
        :invalid-feedback="passwordErrors[0]"
        :state="this.$v.password.$dirty ? passwordErrors.length == 0 : null"
    >
      <b-form-input
        id="passwordInput"
        type="password"
        name="password"
        v-model="password"
        placeholder="Enter new password"
        :state="this.$v.password.$dirty ? passwordErrors.length == 0 : null"
        v-on:input="this.$v.password.$touch"
        :disabled="isChangingPassword" />
    </b-form-group>
    <b-form-group
        id="confirmPasswordInputGroup"
        label="Confirm new password: "
        label-for="confirmPasswordInput"
        :invalid-feedback="confirmPasswordErrors[0]"
        :state="this.$v.confirmPassword.$dirty ? confirmPasswordErrors.length == 0 : null"
    >
      <b-form-input
        id="confirmPasswordInput"
        type="password"
        name="confirmPassword"
        v-model="confirmPassword"
        placeholder="Re-enter new password"
        :state="this.$v.confirmPassword.$dirty ? confirmPasswordErrors.length == 0 : null"
        v-on:input="this.$v.confirmPassword.$touch"
        v-bind:disabled="!this.$v.password.$dirty || this.$v.password.$error || isChangingPassword" />
    </b-form-group>

		<div align="right" class="form-buttons">
			<span class="error-message">{{ errorMessage }}</span>
			<b-button type="button" v-on:click="hide()" variant="outline-danger" class="cancel-button" 
								v-bind:disabled="isChangingPassword">Cancel</b-button>
			<b-button type="submit" variant="outline-success"
								v-bind:disabled="isChangingPassword || !canSubmit">
								<b-spinner v-if="isChangingPassword" small class="spinner" type="grow" />
								Change password
			</b-button>
		</div>
	</b-form>
</b-modal>
</template>

<script>
import { required, minLength, sameAs } from 'vuelidate/lib/validators';

export default {
	name: 'ChangePasswordModal',
	props: {
		isChangingPassword: Boolean,
		errorMessage: String
	},
  data() {
		return {
			currentPassword: '',

			password: '',
      confirmPassword: '',

			showModal: false
    }
  },
  methods: {
    show() {
      this.showModal = true;
		},
		hide() {
			this.showModal = false;
		},
		onSubmit(evt) {
      evt.preventDefault();

      this.$emit('change-password-performed', {
				currentPassword: this.currentPassword,
				newPassword: this.password
			});
		},
    modalShown() {
      this.$refs.currentPasswordBox.focus();
    },
		modalHidden() {
			this.currentPassword = '';
			this.password = '';
			this.confirmPassword = '';
			this.$v.$reset();

			this.$emit('modal-closed');
		},
    modalHiding(evt){
      if (this.isDeletingAccount) {
        evt.preventDefault();
      }
    },
	},
	validations: {
    currentPassword: {
      required
		},
		password: {
      required,
      minLength: minLength(8)
    },
    confirmPassword: {
      sameAsPassword: sameAs('password')
    }
	},
	computed: {
		currentPasswordErrors() {
			var errors = [];

			!this.$v.currentPassword.required && errors.push('Current password is required.');
			
			return errors;
		},
		passwordErrors() {
      var errors = [];
      if (!this.$v.password.$dirty) {
        return errors;
      }

      !this.$v.password.required && errors.push('New password is required.');
      !this.$v.password.minLength && errors.push('Your new password must be at least 8 characters long.');
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
		canSubmit() {
			return !this.$v.$error && this.$v.$dirty;
		}
	}
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.form-buttons {
  margin: 20px 0px 0px 0px;
}

.cancel-button {
  margin: 0px 10px 0px 0px;
}

.lower-opacity {
  opacity: 0.9;
}

.error-message {
  margin: 0px 20px 0px 0px;
  color: #fd5454;
}

#danger-icon {
  color: #fd5454
}

#danger-text {
  margin: 20px 0px 20px 0px;
}
</style>
