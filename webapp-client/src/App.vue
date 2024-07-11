<template>
  <div id="app">
    <Navbar v-bind:isGettingUserDetails="isGettingUserDetails" v-bind:isLoggedIn="isLoggedIn"
            v-bind:username="currentUser.username" 
            v-on:login-click="openLoginModal" v-on:register-click="openRegisterModal"
            v-on:logout-click="handleLogout" v-bind:isLoggingOut="isLoggingOut"
            v-bind:isServerDown="isServerDown" v-on:delete-account="handleDeleteAccountRequest"
            v-on:change-password="handlePasswordChangeRequest" />

    <!-- Modals -->
    <LoginModal ref="loginModalRef" v-bind:errorMessage="loginModalErrorMessage"
                v-on:login-performed="attemptLogin" v-bind:isLoggingIn="isLoggingIn"
                v-on:modal-closed="loginModalClosed" />

    <RegisterModal ref="registerModalRef" v-on:server-down="receivedServerDown"
                   v-on:register-performed="attemptRegister" v-bind:isRegistering="isRegistering"
                   v-bind:successfulRegister="successfulRegister" v-on:modal-closed="registerModalClosed"
                   v-bind:errorMessage="registerModalErrorMessage" />

    <SaveNewBlockModal ref="saveNewBlockModal" v-bind:isSaving="isSaving"
                       v-bind:errorMessage="saveNewBlockModalErrorMessage"
                       v-on:modal-closed="this.saveNewBlockModalErrorMessage = ''"
                       v-on:save-performed="saveNewBlock" />
    <DiscardConfirmationModal ref="discardConfirmationModal"
                              v-on:revert-confirmed="confirmRevert"
                              v-on:clean-confirmed="confirmClean"
                              v-on:blockload-confirmed="confirmBlockLoad"
                              v-on:logout-confirmed="confirmLogout"
                              v-on:delete-confirmed="deleteBlock" />

    <EditSavedBlockModal ref="editSavedBlockModal"
                         v-on:save-performed="editBlock" />

    <DeleteAccountModal ref="deleteAccountModal" v-on:delete-account-performed="deleteAccount"
                        v-bind:isDeletingAccount="isDeletingAccount" v-bind:errorMessage="deleteAccountModalErrorMessage"
                        v-on:modal-closed="deleteAccountModalErrorMessage = ''" />

    <ChangePasswordModal ref="changePasswordModal" v-on:change-password-performed="changePassword"
                         v-bind:isChangingPassword="isChangingPassword" v-bind:errorMessage="changePasswordErrorMessage"
                         v-on:modal-closed="changePasswordErrorMessage = ''" />
    <!------------>

    <div v-if="isServerDown" id="server-down-container" align="center">
      <fa-icon :icon="['far', 'frown-open']" size="10x" spin></fa-icon>
      <h1 id="server-down-message">Sorry, the server seems to be down.</h1>
    </div>
    <div v-else class="d-flex" id="wrapper" v-bind:class="{ toggled: sidebarVisible }">
      <div id="sidebar-wrapper">
        <Sidebar v-bind:sampleBlocks="sampleBlocks" v-bind:savedBlocks="savedBlocks"
                 v-bind:savedBlocksLoading="savedBlocksLoading" 
                 v-bind:sampleBlocksLoading="sampleBlocksLoading"
                 v-bind:isLoggedIn="isLoggedIn" 
                 v-bind:loadedBlockId="currentlyLoadedBlock.id"
                 v-bind:codeLoading="codeLoading"
				 v-on:close-sidebar="toggleSidebar"
                 v-on:load-block="handleBlockLoad"
                 v-on:delete-block="handleBlockDeleteRequest"
                 v-on:edit-block="handleBlockEditRequest" />
      </div>
      <div id="code-area-container">
        <div id="code-area-navbar">
          <CodeAreaNavbar v-on:sidebar-toggle="toggleSidebar"
                          v-bind:sidebarVisible="sidebarVisible"
                          v-bind:changesMadeSinceSave="changesMadeSinceSave"
                          v-bind:isSampleBlock="currentlyLoadedBlock.isSampleBlock"
                          v-on:create-new="handleCreateNewRequest"
                          v-on:discard-changes="handleDiscardChangesRequest"
                          v-on:save-changes="handleSaveChangesRequest"
                          v-on:execute-code="executeCode"
                          v-bind:isSaving="isSaving" v-bind:codeLoading="codeLoading"
                          v-bind:transpileInProgress="transpileInProgress"
                          v-bind:executionInProgress="executionInProgress" />
        </div>
        <b-progress :value="showProgressBar ? 100 : 0" striped animated></b-progress>
		<CodeEditor :codeLoading="codeLoading" ref="codeEditor" v-on:code-changed="changesMadeSinceSave = true" />
        <div id="code-output">
          <CodeOutput v-bind:executionOutputLines="executionOutputLines" v-bind:executionInProgress="executionInProgress"
                      v-bind:transpileErrors="transpileErrors" v-on:clear-output="clearOutput"
                      v-bind:syntaxErrors="syntaxErrors" ref="codeOutput" />
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import Navbar from './components/Navbar.vue';
import Sidebar from './components/Sidebar.vue';

import LoginModal from './components/LoginModal.vue';
import RegisterModal from './components/RegisterModal.vue';
import SaveNewBlockModal from './components/SaveNewBlockModal.vue';
import DiscardConfirmationModal from './components/DiscardConfirmationModal.vue';
import EditSavedBlockModal from './components/EditSavedBlockModal.vue';
import DeleteAccountModal from './components/DeleteAccountModal.vue';
import ChangePasswordModal from './components/ChangePasswordModal.vue';

import CodeAreaNavbar from './components/CodeAreaNavbar.vue';
import CodeEditor from './components/CodeEditor.vue';
import CodeOutput from './components/CodeOutput.vue';

import AuthApi from './services/api/Auth.js';
import UsersApi from './services/api/Users.js';
import SylvreBlocksApi from './services/api/SylvreBlocks.js';
import TranspilerApi from './services/api/Transpiler.js';

export default {
  name: 'app',
  components: {
    Navbar,
    Sidebar,

    LoginModal,
    RegisterModal,
    DiscardConfirmationModal,
    SaveNewBlockModal,
    EditSavedBlockModal,
    DeleteAccountModal,
    ChangePasswordModal,

    CodeAreaNavbar,
    CodeEditor,
    CodeOutput
  },
  data() {
    return {
      isServerDown: false,
      isLoggedIn: false,
      
      isGettingUserDetails: false,
      currentUser: {
        id: null,
        username: '',
        email: '',
        fullName: ''
      },

      loginModalErrorMessage: '',
      isLoggingIn: false,
      isLoggingOut: false,

      registerModalErrorMessage: '',
      isRegistering: false,
      successfulRegister: false,

      sidebarHidden: window.innerWidth <= 768,

      sampleBlocks: [],
      sampleBlocksLoading: true,
      savedBlocks: [],
      savedBlocksLoading: false,

      codeLoading: false,
      currentlyLoadedBlock: {},

      changesMadeSinceSave: false,
      isSaving: false,
      saveNewBlockModalErrorMessage: '',

      transpileInProgress: false,
      executionInProgress: false,
      executionOutputLines: [],
      syntaxErrors: [],
      transpileErrors: [],

      isDeletingAccount: false,
      deleteAccountModalErrorMessage: '',

      isChangingPassword: false,
      changePasswordErrorMessage: ''
    }
  },
  methods: {
    receivedServerDown() {
      this.closeLoginModal();
      this.closeRegisterModal();

      this.isServerDown = true;
    },
    openLoginModal() {
      this.$refs.loginModalRef.show();
    },
    closeLoginModal() {
      this.$refs.loginModalRef.hide();
    },
    loginModalClosed(){
      this.loginModalErrorMessage = '';
    },
    openRegisterModal() {
      this.$refs.registerModalRef.show();
    },
    closeRegisterModal() {
      this.$refs.registerModalRef.hide();
    },
    registerModalClosed() {
      this.registerModalErrorMessage = '';
      this.successfulRegister = false;
    },
    openSettings() {
      
    },
    handleLogout() {
      if (this.changesMadeSinceSave) {
        if (this.currentlyLoadedBlock.id != null && !this.currentlyLoadedBlock.isSampleBlock) {
          this.$refs.discardConfirmationModal.confirmForDiscardAndLogout();
          return;
        }
      }

      this.logout();
    },
    async logout() {
      if (this.currentlyLoadedBlock.id != null && !this.currentlyLoadedBlock.isSampleBlock) {
        this.createNew();
        this.clearOutput();
      }

      this.isLoggingOut = true;

      try {
        await AuthApi.logout();

        this.isLoggedIn = false;
        this.savedBlocks = [];
      }
      catch (error){
        if (!error.response) {
          this.isServerDown = true;
        }
      }
      finally {
        this.isLoggingOut = false;
      }
    },
    async attemptLogin(credentials) {
      this.loginModalErrorMessage = '';
      this.isLoggingIn = true;

      try {
        await AuthApi.login(credentials.usernameOrEmail, credentials.password);
        this.isLoggingIn = false;
        this.closeLoginModal();

        this.isGettingUserDetails = true;

        var user = await UsersApi.getUserByIdentity();
        this.currentUser.id = user.id;
        this.currentUser.username = user.username;
        this.currentUser.email = user.email;
        this.currentUser.fullName = user.fullName;

        this.isGettingUserDetails = false;
        this.isLoggedIn = true;

        this.getAndPopulateSavedBlocks();
      }
      catch(error) {
        if (error.response) {
          this.loginModalErrorMessage = error.response.status === 401 ? 
            'Username or password is incorrect.' : 'Unknown error encountered with login.'
        }
        else {  // server down
          this.closeLoginModal();

          this.isServerDown = true;
        }

        this.isLoggingIn = false;
        this.isGettingUserDetails = false;
      }
    },
    async attemptRegister(details) {
      this.isRegistering = true;

      try {
        await UsersApi.registerUser(details.username, details.password, details.email, '');
        this.successfulRegister = true;
      }
      catch(error) {
        if (error.response) {
          this.registerModalErrorMessage = 'Unknown error encountered.';
        }
        else {  // server down
          this.closeRegisterModal();

          this.isServerDown = true;
        }
      }
      finally {
        this.isRegistering = false;
      }
    },
    async getAndPopulateSavedBlocks() {
      this.savedBlocksLoading = true;

      var savedBlocks = await SylvreBlocksApi.getAllSylvreBlocks(true);
      this.savedBlocks = savedBlocks;

      this.savedBlocksLoading = false;
    },
    handleDiscardChangesRequest() {
      if (this.currentlyLoadedBlock.id != null) {
        this.$refs.discardConfirmationModal.confirmForDiscardAndRevert();
      }
      else {
         this.$refs.discardConfirmationModal.confirmForDiscardAndClean();
      }
    },
    confirmRevert() {
      this.$refs.codeEditor.setNewCode(this.currentlyLoadedBlock.body);
      this.changesMadeSinceSave = false;
    },
    confirmClean() {
      this.createNew();
    },
    confirmBlockLoad(blockToLoad) {
      this.loadBlock(blockToLoad);
    },
    confirmLogout() {
      this.logout();
    },
    async loadBlock(newBlockToLoad) {
      try {
        this.codeLoading = true;
        var newBlock = {};
        if (newBlockToLoad.isSampleBlock) {
          newBlock = await SylvreBlocksApi.getSampleSylvreBlockById(newBlockToLoad.id);
        }
        else {
          newBlock = await SylvreBlocksApi.getSylvreBlockById(newBlockToLoad.id);
        }

        this.currentlyLoadedBlock = newBlock;
        this.$refs.codeEditor.setNewCode(newBlock.body);
        this.changesMadeSinceSave = false;
      }
      catch(error) {
        if (!error.response) {
          this.isServerDown = true;
        }
      }
      finally {
        this.codeLoading = false;
      }
    },
    handleSaveChangesRequest() {
      if (!this.isLoggedIn) {
        this.openLoginModal();
      }
      else if (this.currentlyLoadedBlock.id == null || this.currentlyLoadedBlock.isSampleBlock) {
        this.$refs.saveNewBlockModal.show();
      }
      else {
        this.saveCurrentBlock();
      }
    },
    async saveNewBlock(blockName) {
      try {
        this.isSaving = true;
        var currentCode = this.$refs.codeEditor.getCode();

        this.currentlyLoadedBlock = await SylvreBlocksApi.createSylvreBlock(blockName, currentCode);
        this.savedBlocks.unshift(this.currentlyLoadedBlock);
        this.changesMadeSinceSave = false;
        this.$refs.saveNewBlockModal.hide();
      }
      catch(error) {
        if (error.response) {
          this.saveNewBlockModalErrorMessage = 'Unknown error encountered.';
        }
        else {
          this.$refs.saveNewBlockModal.hide();

          this.isServerDown = true;
        }
      }
      finally {
        this.isSaving = false;
      }
    },
    async saveCurrentBlock() {
      try {
        this.isSaving = true;

        var updatedCode = this.$refs.codeEditor.getCode();
        await SylvreBlocksApi.updateSylvreBlockById(this.currentlyLoadedBlock.id, null, updatedCode);

        this.currentlyLoadedBlock.body = updatedCode;
        this.changesMadeSinceSave = false;
      }
      catch(error) {
        if (!error.response) {
          this.isServerDown = true;
        }
      }
      finally {
        this.isSaving = false;
      }
    },
    handleCreateNewRequest() {
      if (this.changesMadeSinceSave) {
        this.$refs.discardConfirmationModal.confirmForDiscardAndClean();
      }
      else {
        this.createNew();
      }
    },
    createNew() {
      this.currentlyLoadedBlock = {};
      this.$refs.codeEditor.setNewCode('');
      this.changesMadeSinceSave = false;
    },
    handleBlockDeleteRequest(blockIdToDelete) {
      this.$refs.discardConfirmationModal.confirmForDelete(blockIdToDelete);
    },
    async deleteBlock(blockIdToDelete) {
      if (this.currentlyLoadedBlock.id == blockIdToDelete) {
        this.createNew();
      }

      this.savedBlocksLoading = true;

      try {
        await SylvreBlocksApi.deleteSylvreBlockById(blockIdToDelete);

        for (var i = 0; i < this.savedBlocks.length; i++) {
          if (this.savedBlocks[i].id == blockIdToDelete) {
            this.savedBlocks.splice(i, 1);
          }
        }
      }
      catch(error) {
        if (!error.response) {
          this.isServerDown = true;
        }
      }

      this.savedBlocksLoading = false;
    },
    handleBlockEditRequest(blockToEdit) {
      this.$refs.editSavedBlockModal.show(blockToEdit);
    },
    async editBlock(editedBlock) {
      this.codeLoading = true;
      try {
        await SylvreBlocksApi.updateSylvreBlockById(editedBlock.id, editedBlock.name, null);

        for (var i = 0; i < this.savedBlocks.length; i++) {
          if (this.savedBlocks[i].id == editedBlock.id) {
            this.savedBlocks[i].name = editedBlock.name;
          }
        }
      }
      catch(error) {
        if (!error.response) {
          this.isServerDown = true;
        }
      }
      this.codeLoading = false;
    },
    handleBlockLoad(blockToLoad) {
      if (this.changesMadeSinceSave) {
        this.$refs.discardConfirmationModal.confirmForDiscardAndBlockLoad(blockToLoad);
      }
      else {
        this.loadBlock(blockToLoad);
      }
    },
    clearOutput() {
      this.syntaxErrors = [];
      this.transpileErrors = [];
      this.executionOutputLines = [];
    },
    async executeCode() {
      this.$refs.codeOutput.clearOutput();

      var jsCodeToExecute = ''; 
      try {
        this.transpileInProgress = true;

        var codeToTranspile = this.$refs.codeEditor.getCode();
        var transpileResult = await TranspilerApi.transpileCode(codeToTranspile, 'JavaScript');

        if (transpileResult.hasErrors) {
          if (transpileResult.errorSource == 'Parser') {
            this.syntaxErrors = transpileResult.errors;
          }
          else if (transpileResult.errorSource == 'Transpiler'){
            this.transpileErrors = transpileResult.errors;
          }

          this.transpileInProgress = false;
          return;
        }

        this.transpileInProgress = false;
        jsCodeToExecute = transpileResult.transpiledCode;
      }
      catch(error) {
        if (!error.response) {
          this.isServerDown = true;
        }
      }

      this.executionInProgress = true;
      this.$nextTick(() => {
        try {
          this.jsExecutor(jsCodeToExecute);
        }
        catch(error) {
          this.executionOutputLines.push({
            isError: true,
            text: 'Execution error: ' + error
          });
        }
        
        this.executionInProgress = false;
        this.$refs.codeOutput.finishExecution();
      });
    },
    jsExecutor(jsCode) {
      return Function(jsCode)();
    },
    consoleLogOverrider: function(msg) {
      this.executionOutputLines.push({
        isError: false,
        text: msg
      });
    },
    consoleClearOverrider: function() {
      this.executionOutputLines = this.executionOutputLines.filter((line) => {
        return line.isError;
      })
    },
    handleDeleteAccountRequest() {
      this.$refs.deleteAccountModal.show();
    },
    async deleteAccount(currentPassword) {
      this.deleteAccountModalErrorMessage = '';
      this.isDeletingAccount = true;

      try {
        var base64CurrentPassword = window.btoa(currentPassword);
        await UsersApi.deleteUser(this.currentUser.id, base64CurrentPassword);

        if (this.currentlyLoadedBlock.id != null && !this.currentlyLoadedBlock.isSampleBlock) {
          this.createNew();
          this.clearOutput();
        }

        this.isLoggedIn = false;
        this.savedBlocks = [];
        this.$refs.deleteAccountModal.hide();
      }
      catch(error) {
        if (!error.response) {
          this.isServerDown = true;
          this.$refs.deleteAccountModal.hide();
        }
        else {
          this.deleteAccountModalErrorMessage = error.response.status === 400 ? 
            'Password is incorrect.' : 'Unknown error encountered.';
        }
      }
      finally {
        this.isDeletingAccount = false;
      }
    },
    handlePasswordChangeRequest() {
      this.$refs.changePasswordModal.show();
    },
    async changePassword(data) {
      this.changePasswordErrorMessage = '';
      this.isChangingPassword = true;

      try {
        var base64CurrentPassword = window.btoa(data.currentPassword);
        await UsersApi.changeUserPassword(this.currentUser.id,
          base64CurrentPassword, data.newPassword);

        this.$refs.changePasswordModal.hide();
      }
      catch(error) {
        if (!error.response) {
          this.isServerDown = true;
          this.$refs.changePasswordModal.hide();
        }
        else {
          this.changePasswordErrorMessage = error.response.status === 400 ? 
            'Current password is incorrect.' : 'Unknown error encountered.';
        }
      }
      finally {
        this.isChangingPassword = false;
      }
    },
    toggleSidebar() {
      this.sidebarHidden = !this.sidebarHidden;
    }
  },
  created: async function() {
    window.console.log = this.consoleLogOverrider;
    window.console.clear = this.consoleClearOverrider;

    try {
      this.isGettingUserDetails = true;
      
      var user = await UsersApi.getUserByIdentity();
      this.currentUser.id = user.id;
      this.currentUser.username = user.username;
      this.currentUser.email = user.email;
      this.currentUser.fullName = user.fullName;

      this.isLoggedIn = true;
      this.isGettingUserDetails = false;
      this.getAndPopulateSavedBlocks();
    }
    catch(error) {
      if (!error.response) {
        this.isServerDown = true;
      }
      
      this.isLoggedIn = false;
    }
    finally {
      this.isGettingUserDetails = false;
    }

    try {
      var sampleBlocks = await SylvreBlocksApi.getAllSampleSylvreBlocks(true);
      this.sampleBlocks = sampleBlocks;
      this.sampleBlocksLoading = false;

      if (sampleBlocks.length != 0) {
        this.loadBlock(sampleBlocks[0]);
      }
    }
    catch(error) {
      if (!error.response) {
        this.isServerDown = true;
      }
    }
  },
  computed: {
    showProgressBar() {
      return this.codeLoading || this.transpileInProgress || this.executionInProgress;
    },
	sidebarVisible() {
      return !this.sidebarHidden;
    }
  }
}
</script>

<style>
html, body, #app, #wrapper {
  height: 100%;
  margin: 0;
}

#app {
  font-family: 'Roboto', 'Avenir', Helvetica, Arial, sans-serif;
  display: flex;
  flex-direction: column;
}

#server-down-container {
  margin: 200px 0px 0px 0px;
}

@media only screen and (max-width: 768px){
  #server-down-container {
    margin: 30px 0px 0px 0px;
  }
}

#server-down-message {
  margin: 20px 0px 0px 0px;
}

#wrapper {
  display: flex;
  flex: 1;
}

#sidebar-wrapper {
  flex: 0 0 250px;
  display: flex;
  flex-direction: column;
  height: 100%;
  border-right: 1px solid rgba(117, 77, 235, 0.651);
  -webkit-transition: margin .25s ease-out;
  -moz-transition: margin .25s ease-out;
  -o-transition: margin .25s ease-out;
  transition: margin .25s ease-out;
  margin-left: -321px;
}
#wrapper.toggled #sidebar-wrapper {
  margin-left: 0px;
}

@media only screen and (max-width: 768px) {
  #sidebar-wrapper {
    flex: 0 0 193px;
	margin-left: -193px;
  }

  #wrapper.toggled #sidebar-wrapper {
    margin-left: 0px;
  }
}

#code-area-container {
  display: flex;
  flex-direction: column;
  flex: 1;
  min-width: 0;
  height: 100%;
}

#code-area-navbar {
  flex: 0 0 auto;
}

#code-editor {
  flex: 1 1 auto;
  overflow: auto;
  display: flex;
  flex-direction: column;
}

#code-output {
  flex: 0 0 auto;
}

.progress-bar {
  background-color: #482C9C;
}
</style>