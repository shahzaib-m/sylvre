<template>
<div>
	<b-navbar class="navbar" type="dark">
		Output:
		<b-navbar-nav class="ml-auto">
			<b-button variant="outline-warning" size="sm" v-on:click="clearOutput">Clear</b-button>
		</b-navbar-nav>
	</b-navbar>
	<div id="output" ref="output">
		<p v-for="(syntaxError, index) in syntaxErrors" v-bind:key="index" class="syntax-error">
			Syntax error: Line {{ syntaxError.line }} | "{{ syntaxError.symbol }}" | {{ syntaxError.message }}
		<p>
		<p v-for="(transpileError, index) in transpileErrors" v-bind:key="index" class="transpile-error">
			Transpile Error: Line {{ transpileError.line }} | "{{ transpileError.symbol }}" | {{ transpileError.message }}
		</p>
		<p v-for="(executionOutputLine, index) in executionOutputLines" v-bind:key="index"
			 v-bind:class="{ 'error-line': executionOutputLine.isError }">
			{{ executionOutputLine.text }}
		</p>
		<p class="finish-line" v-if="executionFinished">Execution finished.</p>
	</div>
</div>
</template>

<script>
export default {
	name: 'CodeOutput',
	props: {
		executionOutputLines: Array,
		transpileErrors: Array,
		syntaxErrors: Array
	},
	watch: {
		transpileErrors() {
			this.scrollToEnd();
		},
		executionOutputLines() {
			this.scrollToEnd();
		}
	},
	data() {
		return {
			executionFinished: false
		}
	},
	methods: {
		clearOutput() {
			this.$emit('clear-output');
			this.executionFinished = false;
		},
		finishExecution() {
			this.executionFinished = true;
		},
		scrollToEnd() {
			this.$nextTick(() => {
				var outputElement = this.$refs.output;
				outputElement.scrollTop = outputElement.scrollHeight;
			});
		}
	}
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.navbar {
	background-color: rgb(58, 42, 107);
	color: #e9e9e9;
	height: 35px;

	padding: 1rem 5px 1rem 5px;
}

#output {
	overflow-y: auto;

	max-height: 21.5vh;
	min-height: 21.5vh;
	background: #151515;
}

p {
	margin: 0px 0px 0px 5px;
	font-family: 'Roboto Mono', monospace;
}

.error-line {
	color: #ff6a6a
}

.finish-line {
	color: #7cff82;
	margin: 15px 0px 10px 5px;
}

.syntax-error {
	color: #f8aa50 
}

.transpile-error {
	color: rgb(214, 179, 253)
}
</style>
