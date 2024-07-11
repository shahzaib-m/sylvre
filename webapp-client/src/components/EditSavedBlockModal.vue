<template>
<b-modal v-model="showModal" title="Edit block" hide-header-close hide-footer
				 v-on:shown="modalShown" v-on:hidden="modalHidden">
	<b-form @submit="onSubmit">
		<b-form-group
			id="blocknameInputGroup"
			label="Edit block name: "
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
			v-on:input="this.$v.blockname.$touch" />
		</b-form-group>

		<div align="right" class="form-buttons">
			<b-button type="button" v-on:click="hide()" variant="outline-danger" class="cancel-button">
				Cancel
			</b-button>
			<b-button type="submit" variant="outline-success"
								v-bind:disabled="!canSubmit">
								Save
			</b-button>
		</div>
	</b-form>
</b-modal>
</template>

<script>
import { required, maxLength } from 'vuelidate/lib/validators';

export default {
	name: 'EditSavedBlockModal',
  data() {
		return {
			blockId: null,
			blockname: 'null',
			showModal: false
    }
  },
  methods: {
    show(blockToEdit) {
			this.blockId = blockToEdit.id;
			this.blockname = blockToEdit.name;

      this.showModal = true;
		},
		hide() {
			this.showModal = false;
		},
		onSubmit(evt) {
      evt.preventDefault();

      this.$emit('save-performed', {
				id: this.blockId,
				name: this.blockname
			});

			this.hide();
		},
    modalShown() {
      this.$refs.blocknameBox.focus();
    },
		modalHidden() {
			this.blockId = null;
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
</style>
