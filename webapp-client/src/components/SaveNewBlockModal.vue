<template>
<b-modal v-model="showModal" title="Save new block" hide-header-close hide-footer
				 v-on:shown="modalShown" v-on:hidden="modalHidden">
	<b-form @submit="onSubmit" v-bind:class="{ 'lower-opacity': isSaving }">
		<b-form-group
			id="blocknameInputGroup"
			label="Block name: "
			label-for="blocknameInput"
			:invalid-feedback="blocknameErrors[0]"
			:state="this.$v.blockname.$dirty ? blocknameErrors.length == 0 : null"
		>
		<b-form-input
			id="blocknameInput"
			ref="blocknameBox"
			type="text"
			name="blockname"
			v-model="blockname"
			placeholder="Enter block name"
			:state="this.$v.blockname.$dirty ? blocknameErrors.length == 0 : null"
			v-on:input="this.$v.blockname.$touch"
			:disabled="isSaving" />
		</b-form-group>

		<div align="right" class="form-buttons">
			<span class="error-message">{{ errorMessage }}</span>
			<b-button type="button" v-on:click="hide()" variant="outline-danger" class="cancel-button" 
								v-bind:disabled="isSaving">Cancel</b-button>
			<b-button type="submit" variant="outline-success"
								v-bind:disabled="isSaving || !canSubmit">
								<b-spinner v-if="isSaving" small class="spinner" type="grow" />
								Save
			</b-button>
		</div>
	</b-form>
</b-modal>
</template>

<script>
import { required, maxLength } from 'vuelidate/lib/validators';

export default {
	name: 'SaveNewBlockModal',
	props: {
		isSaving: Boolean,
		errorMessage: String
	},
  data() {
		return {
			blockname: '',
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

      this.$emit('save-performed', this.blockname);
		},
    modalShown() {
      this.$refs.blocknameBox.focus();
    },
		modalHidden() {
			this.blockname = '';

			this.$v.$reset();
		}
	},
	validations: {
    blockname: {
      required,
      maxLength: maxLength(50)
    }
	},
	computed: {
		blocknameErrors() {
			var errors = [];

			!this.$v.blockname.required && errors.push('Block name is required.');
			!this.$v.blockname.maxLength && errors.push('Block name must not be more than 50 characters in length.');
			
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
  margin: 30px 0px 0px 0px;
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
</style>
