<template>
  <b-navbar class="navbar" toggleable="xl" type="dark">
    <b-navbar-brand href="#">
      <img class="d-inline-block align-top" src="../assets/logo-250.png" width="25" height="25" alt="">
      Sylvre Web Editor
    </b-navbar-brand>
     <b-navbar-toggle target="nav_collapse" />

    <b-collapse is-nav id="nav_collapse">
      <b-navbar-nav>
        <b-nav-item class="nav-item" href="/swagger/index.html" target="_blank">
          Interactive API
          <fa-icon :icon="['fas', 'code']" />
        </b-nav-item>
        <b-nav-item class="nav-item" href="https://github.com/shahzaib-m/sylvre" target="_blank">
          Source code
          <fa-icon :icon="['fab', 'github']"></fa-icon>
        </b-nav-item>
        <b-nav-item class="nav-item" href="https://shahzaibm.com" target="_blank">
          by shahzaib-m
        </b-nav-item>
      </b-navbar-nav>
      <b-navbar-nav class="ml-auto">
        <div v-if="isLoadingUser">
           <b-spinner type="grow" variant="success" />
        </div>
        <div v-else-if="isLoggedIn" class="not-logged-in animated infinite fadeIn">
          <b-dropdown v-bind:text="username" variant="outline-success" class="settings-button"
                      v-bind:disabled="isServerDown">
            <b-dropdown-item v-on:click="$emit('change-password')">
              <fa-icon icon="key"></fa-icon>
              Change password
            </b-dropdown-item>
           <b-dropdown-divider />
            <b-dropdown-item v-on:click="$emit('delete-account')">
              <fa-icon icon="trash-alt"></fa-icon>
              Delete account
            </b-dropdown-item>
          </b-dropdown>
          <b-button variant="outline-warning" type="button"
                    @click="$emit('logout-click')" v-bind:disabled="isServerDown">
            <fa-icon icon="sign-out-alt"></fa-icon>
            Logout
          </b-button>
        </div>
        <div v-else class="not-logged-in animated infinite fadeIn">
          <b-button variant="outline-success" class="sign-in-button" type="button"
                    @click="$emit('login-click')" v-bind:disabled="isServerDown">
            <fa-icon icon="sign-in-alt"></fa-icon>
            Login
          </b-button>
          <b-button variant="outline-info" type="button"
                    @click="$emit('register-click')" v-bind:disabled="isServerDown">
            <fa-icon icon="user-plus"></fa-icon>
            Register
          </b-button>
        </div>
      </b-navbar-nav>
     </b-collapse>
  </b-navbar>
</template>

<script>
export default {
  name: 'Navbar',
  props: {
    isServerDown: Boolean,
    isAttemptingRefreshLogin: Boolean,
    isLoggedIn: Boolean,
    isGettingUserDetails: Boolean,
    isLoggingOut: Boolean,
    username: String
  },
  computed: {
    isLoadingUser: function() {
      return this.isAttemptingRefreshLogin || this.isGettingUserDetails || this.isLoggingOut;
    }
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.navbar {
  background-color: #482C9C;
  z-index: 1000;
}

.navbar-dark .navbar-nav .nav-link {
  color: rgb(199, 180, 255); 
  opacity: 0.5;
  transition: .3s;
}

.navbar-dark .navbar-nav .nav-link:hover {
  color: #ffffff;
  opacity: 1;
}

.sign-in-button {
  margin: 0px 10px 0px 0px
}

.settings-button {
  margin: 0px 10px 0px 0px
}

#username {
  color: rgb(234, 234, 234);
  margin: 0px 15px 0px 0px;
  vertical-align: middle;
  font-weight: bold
}

.not-logged-in {
  animation-duration: 0.4s;
  animation-iteration-count: 1;
}

.logged-in {
  animation-duration: 0.4s;
  animation-iteration-count: 1;
}

@media only screen and (min-width: 1200px) {
  .navbar {
    max-height: 55px;
  }
}
</style>
