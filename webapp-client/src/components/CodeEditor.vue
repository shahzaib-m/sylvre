<template>
   <codemirror :disabled="codeLoading" ref="cmInstance" v-model="code" :options="cmOptions"
	 						 @input="onCmCodeChange"></codemirror>
</template>

<script>
export default {
	name: 'CodeEditor',
	props: {
		codeLoading: Boolean
	},
  data () {
    return {
      code: '',
      newCodeReceived: false,
      cmOptions: {
        tabSize: 2,
        mode: 'sylvre',
        lineNumbers: true,
        line: true
      }
    }
	},
  methods: {
    setNewCode(code) {
      this.code = code;
      this.newCodeReceived = true;

      this.$nextTick(() => {  // need to wait for DOM to update
        this.codemirror.doc.clearHistory(); // remove undo history
      });
    },
    getCode() {
      return this.code;
    },
    onCmCodeChange() {
      if (this.newCodeReceived) { // change event fired due to setNewCode() code update
        this.newCodeReceived = false;
      }
      else {  // change event fired due to user input code update
        this.$emit('code-changed');
      }
    }
  },
  computed: {
    codemirror() {
      return this.$refs.cmInstance.codemirror
		}
  },
  mounted() {
    
  }
}
</script>
<style>
.vue-codemirror {
  line-height: 1.5em;
  flex: 1 1 auto;
  margin-top: 0;
  height: 100%;
  position: relative;
}

.CodeMirror {
	position: absolute;
	top: 0;
	bottom: 0;
	left: 0;
	right: 0;
	height: 100%;
	font-size: 17px;
	font-family: 'Roboto Mono', monospace !important;
}

@media only screen and (max-width: 768px) {
  .vue-codemirror, .CodeMirror {
    font-size: 15px;
  }
}

/** custom/modified token styles **/
.cm-special {
  color: rgb(137, 129, 255);
}

.cm-number {
  color: #2dc78c !important;
}
</style>