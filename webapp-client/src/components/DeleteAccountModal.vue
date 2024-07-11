<template>
<b-modal v-model="showModal" title="Delete your account" hide-header-close hide-footer
				 v-on:shown="modalShown" v-on:hidden="modalHidden" v-on:hide="modalHiding">
	<div align="center">
    <span id="danger-icon">
      <fa-icon icon="exclamation-triangle" size="7x"></fa-icon>
    </span>
    <div id="danger-text">
      <h4>Danger zone!</h4>
      <h5>Your account and any code blocks saved under it will be deleted. Re-authentication is required.</h5>
    </div>
  </div>
	<b-form @submit="onSubmit" v-bind:class="{ 'lower-opacity': isDeletingAccount }">
		<b-form-group
			id="passwordInputGroup"
			:invalid-feedback="currentPasswordErrors[0]"
			:state="this.$v.currentPassword.$dirty ? currentPassword.length == 0 : null"
		>
		<b-form-input
			id="passwordInput"
			ref="passwordBox"
			type="password"
			name="password"
			v-model="currentPassword"
			placeholder="Enter current password"
			:state="this.$v.currentPassword.$dirty ? currentPasswordErrors.length == 0 : null"
			v-on:input="this.$v.currentPassword.$touch"
			:disabled="isDeletingAccount" />
		</b-form-group>

		<div align="right" class="form-buttons">
			<span class="error-message">{{ errorMessage }}</span>
			<b-button type="button" v-on:click="hide()" variant="outline-danger" class="cancel-button" 
								v-bind:disabled="isDeletingAccount">Cancel</b-button>
			<b-button type="submit" variant="outline-success"
								v-bind:disabled="isDeletingAccount || !canSubmit">
								<b-spinner v-if="isDeletingAccount" small class="spinner" type="grow" />
								Delete account
			</b-button>
		</div>
	</b-form>
</b-modal>
</template>

<script>
import { required } from 'vuelidate/lib/validators';

export default {
	name: 'DeleteAccountModal',
	props: {
		isDeletingAccount: Boolean,
		errorMessage: String
	},
  data() {
		return {
			currentPassword: '',
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

      this.$emit('delete-account-performed', this.currentPassword);
		},
    modalShown() {
      this.$refs.passwordBox.focus();
    },
		modalHidden() {
			this.currentPassword = '';
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
    }
	},
	computed: {
		currentPasswordErrors() {
			var errors = [];

			!this.$v.currentPassword.required && errors.push('Current password is required.');
			
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
