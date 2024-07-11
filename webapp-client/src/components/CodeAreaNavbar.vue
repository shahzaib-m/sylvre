<template>
  <b-navbar class="navbar" toggleable="xl" type="dark">
      <b-button id="sidebar-toggle-button" variant="outline-info" v-b-tooltip.hover
                :title="sidebarVisible ? 'Collapse menu' : 'Show menu'"
                v-on:click="sidebarToggle">
        <fa-icon v-if="!sidebarVisible" icon="chevron-right"></fa-icon>
        <fa-icon v-else icon="chevron-left"></fa-icon>
      </b-button>
    <b-navbar-toggle target="nav_collapse_2" />

    <b-collapse is-nav id="nav_collapse_2">
      <b-navbar-nav>
        <b-nav-item class="nav-item new-button-item" :disabled="codeLoading">
          <b-button id="new-button" variant="outline-dark" v-b-tooltip.hover
                    title="Create a new block" :disabled="codeLoading"
                    v-on:click="createNew">
            <fa-icon icon="plus"></fa-icon>
            New
          </b-button>
        </b-nav-item>
        <b-nav-item class="nav-item" :disabled="codeLoading || (!changesMadeSinceSave && !isSampleBlock)">
          <b-button id="save-button" variant="outline-info" v-b-tooltip.hover
                    title="Save current block" :disabled="codeLoading || (!changesMadeSinceSave && !isSampleBlock)"
                    v-on:click="saveChanges">
            <fa-icon icon="save"></fa-icon>
            Save
            <b-spinner v-if="isSaving" small class="spinner" type="grow" />
          </b-button>
        </b-nav-item>
        <b-nav-item class="nav-item" :disabled="codeLoading || !changesMadeSinceSave">
          <b-button id="discard-button" variant="outline-danger" v-b-tooltip.hover
                    title="Discard changes in block" :disabled="codeLoading || !changesMadeSinceSave"
                    v-on:click="discardChanges">
            <fa-icon icon="trash-alt"></fa-icon>
            Discard
          </b-button>
        </b-nav-item>
      </b-navbar-nav>
      <b-navbar-nav class="ml-auto">
        <b-nav-item class="nav-item" :disabled="codeLoading || transpileInProgress || executionInProgress">
          <b-button align="right" id="execute-button" variant="outline-success" v-b-tooltip.hover
                    title="Execute current block" :disabled="codeLoading || transpileInProgress || executionInProgress"
                    v-on:click="executeCode">
            <fa-icon icon="play"></fa-icon>
            {{ executeButtonText }}
            <b-spinner v-if="transpileInProgress || executionInProgress" small class="spinner" type="grow" />
          </b-button>
        </b-nav-item>
      </b-navbar-nav>
    </b-collapse>
  </b-navbar>
</template>

<script>
export default {
  name: 'CodeAreaNavbar',
  props: {
    sidebarVisible: Boolean,
    
    changesMadeSinceSave: Boolean,
    isSampleBlock: Boolean,

    isSaving: Boolean,
    codeLoading: Boolean,

    transpileInProgress: Boolean,
    executionInProgress: Boolean,
  },
  methods: {
    sidebarToggle() {
			this.$emit('sidebar-toggle');
    },
    createNew() {
      this.$emit('create-new');
    },
    discardChanges() {
      this.$emit('discard-changes');
    },
    saveChanges() {
      this.$emit('save-changes');
    },
    executeCode() {
      this.$emit('execute-code');
    }
  },
  computed: {
    executeButtonText() {
      if (this.transpileInProgress) {
        return 'Transpiling';
      }
      
      if (this.executionInProgress) {
        return 'Executing';
      }

      return 'Execute';
    }
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.navbar {
  background-color: rgb(27, 27, 27);
  padding: 5px 5px 5px 5px;
}

#sidebar-toggle-button {
    color: #8c84ff;
    border-color: #8c84ff
}

@media only screen and (min-width: 1200px) {
  .navbar {
    height: 40px;
  }

  .new-button-item {
    margin: 0px 0px 0px 30px;
  }

  .navbar-nav .nav-link {
    padding-left: 0;
  }
}
</style>
