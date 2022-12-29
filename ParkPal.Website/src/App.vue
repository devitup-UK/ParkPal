<template>
  <div id="app">
      <b-navbar toggleable="lg" class="navigation">
        <b-navbar-brand href="/">
          <img src="@/assets/logo.png" style="height: 36px;" alt="ParkPal Logo" />
        </b-navbar-brand>
        <b-navbar-toggle target="nav-collapse"></b-navbar-toggle>


        <b-collapse id="nav-collapse" is-nav>

          <!-- Right aligned nav items -->
          <b-navbar-nav align="right" class="w-100">

            <b-nav-item href="#download" class="navigation__item">
              <font-awesome-icon fixed-width icon="mobile"></font-awesome-icon>
              <span>DOWNLOAD</span>
            </b-nav-item>
            <b-nav-item href="#themeparks" class="navigation__item">
              <img src="@/assets/park-icon.svg" alt="Theme Parks Icon">
              <span>THEME PARKS</span>
            </b-nav-item>
            <b-nav-item href="#features" class="navigation__item">
              <font-awesome-icon fixed-width icon="star"></font-awesome-icon>
              <span>FEATURES</span>
            </b-nav-item>
            <b-nav-item href="#screenshots" class="navigation__item">
              <font-awesome-icon fixed-width icon="photo-film"></font-awesome-icon>
              <span>APPLICATION</span>
            </b-nav-item>
            <b-nav-item href="#getintouch" class="navigation__item">
              <font-awesome-icon fixed-width icon="at"></font-awesome-icon>
              <span>GET IN TOUCH</span>
            </b-nav-item>
          </b-navbar-nav>
        </b-collapse>

      </b-navbar>

    <b-container class="content" id="download">
      <b-row class="destinations">
        <b-col sm="12" lg="4" class="destinations__image">
            <img src="@/assets/destinations.png" class="w-100" alt="Destinations Screenshot">
        </b-col>
        <b-col sm="12" lg="8" class="destinations__description">
            <h1>ParkPal</h1>
            <h2>The last theme park wait time you’ll ever need.</h2>
            <p>Set target wait time notifications and view wait times for over 50+ destinations worldwide, all in a simple app so you can carry it with you all day while you visit your favourite theme park destination.</p>
            <b-button pill>
              <font-awesome-icon :icon="['brands', 'apple']" fixed-width></font-awesome-icon>
              <span>App Store</span>
            </b-button>
        </b-col>
      </b-row>
    </b-container>

    <b-container fluid class="content" id="themeparks">
      <b-row class="theme-parks">
        <b-col>
          <h1>Popular Theme Parks</h1>
          <b-row class="theme-parks__featured">
            <b-col cols="12" lg="3" offset-lg="1">
              <img src="@/assets/disney.svg" alt="Disney Logo">
            </b-col>
            <b-col cols="12" lg="3" offset-lg="1">
              <img src="@/assets/universal.svg" alt="Universal Logo">
            </b-col>
            <b-col cols="12" lg="3" offset-lg="1">
              <img src="@/assets/six-flags.svg" alt="Six Flags Logo">
            </b-col>
          </b-row>
          <b-row class="theme-parks__more">
            <b-col>
              <p>...and many more!</p>
            </b-col>
          </b-row>
        </b-col>
      </b-row>
    </b-container>

    <b-container class="content mt-4" id="features">
      <b-row>
        <h1>Features</h1>
      </b-row>
      <b-row class="features">
        <b-col sm="12" lg="4" class="feature">
          <img src="@/assets/park-icon.svg" logo="Theme Parks Icon">
          <span>50 + Theme Park Destinations</span>
        </b-col>
        <b-col sm="12" lg="4" class="feature">
          <font-awesome-icon icon="clock" fixed-width></font-awesome-icon>
          <span>Live Wait Times, directly from the theme parks.</span>
        </b-col>
        <b-col sm="12" lg="4" class="feature">
          <font-awesome-icon icon="heart" fixed-width></font-awesome-icon>
          <span>Favourite Attractions to find them easily.</span>
        </b-col>
        <b-col sm="12" lg="4" class="feature">
          <font-awesome-icon icon="bell" fixed-width></font-awesome-icon>
          <span>Set “less than”, “more than” or “equal to” wait time notifications.</span>
        </b-col>
        <b-col sm="12" lg="4" class="feature">
          <font-awesome-icon icon="paint-roller" fixed-width></font-awesome-icon>
          <span>Custom Theming to make the App fit you.</span>
        </b-col>
        <b-col sm="12" lg="4" class="feature">
          <font-awesome-icon icon="search" fixed-width></font-awesome-icon>
          <span>Search & Filter Destinations, Attractions and Notifications.</span>
        </b-col>
      </b-row>
    </b-container>

    <b-container class="content mt-4" id="screenshots">
      <b-row>
        <h1>Screenshots</h1>
      </b-row>
      <b-row class="screenshots">
        <b-col>
          <Swiper :slidesPerView="1" :spaceBetween="50" navigation :modules="modules" :breakpoints="{
            760: {
              slidesPerView: 2
            },
            1200: {
              slidesPerView: 3
            }
           }" class="screenshots-slider">
            <SwiperSlide class="screenshots-slider__screenshot">
              <img src="@/assets/screenshot-destinations.png" alt="Destinations Screenshot">
              <p>Destinations</p>
            </SwiperSlide>
            <SwiperSlide class="screenshots-slider__screenshot">
              <img src="@/assets/screenshot-parks.png" alt="Parks Screenshot">
              <p>Parks</p>
            </SwiperSlide>
            <SwiperSlide class="screenshots-slider__screenshot">
              <img src="@/assets/screenshot-waittimes.png" alt="Wait Times Screenshot">
              <p>Wait Times</p>
            </SwiperSlide>
            <SwiperSlide class="screenshots-slider__screenshot">
              <img src="@/assets/screenshot-notifications.png" alt="Notifications Screenshot">
              <p>Notifications</p>
            </SwiperSlide>
            <SwiperSlide class="screenshots-slider__screenshot">
              <img src="@/assets/screenshot-createanotification.png" alt="Create A Notification Screenshot">
              <p>Create A Notification</p>
            </SwiperSlide>
            <SwiperSlide class="screenshots-slider__screenshot">
              <img src="@/assets/screenshot-settings.png" alt="Settings Screenshot">
              <p>Settings</p>
            </SwiperSlide>

            <SwiperSlide class="screenshots-slider__screenshot">
              <img src="@/assets/screenshot-managedestinations.png" alt="Manage Destinations Screenshot">
              <p>Manage Destinations</p>
            </SwiperSlide>
            <SwiperSlide class="screenshots-slider__screenshot">
              <img src="@/assets/screenshot-parkpalplus.png" alt="ParkPal+ Subscription Screenshot">
              <p>ParkPal+ Subscription</p>
            </SwiperSlide>
            <SwiperSlide class="screenshots-slider__screenshot">
              <img src="@/assets/screenshot-about.png" alt="About & FAQs Screenshot">
              <p>About & FAQ's</p>
            </SwiperSlide>
            <SwiperSlide class="screenshots-slider__screenshot">
              <img src="@/assets/screenshot-theming.png" alt="Custom Theming Screenshot">
              <p>Custom Theming</p>
            </SwiperSlide>
          </Swiper>
        </b-col>
      </b-row>
    </b-container>

    <b-container class="content mt-4 content--theming" fluid id="screenshots">
      <b-row class="theming">
        <b-col cols="12" lg="6" class="theming__dark-mode">
          <h1>Dark Mode</h1>
          <img src="@/assets/screenshot-darkmode.png" alt="Destinations Dark Mode Screenshot">
        </b-col>
        <b-col cols="12" lg="6" class="theming__light-mode">
          <h1>Light Mode</h1>
          <img src="@/assets/screenshot-lightmode.png" alt="Destinations Light Mode Screenshot">
        </b-col>
      </b-row>
    </b-container>

    <b-container class="content mt-5" id="getintouch">
      <b-row>
        <h1>Get In Touch</h1>
      </b-row>
      <b-row v-if="contact.loading">
        <div class="loader">
          <div class="loader__rail"></div>
          <span></span>
          <span></span>
          <span></span>
          <div class="loader__loop" :style="`border: 0.2em solid #000 !important;`"></div>
        </div>
      </b-row>
      <b-row v-if="contact.loading" class="mt-2">
        <b-col>
          <p>Sending message...</p>
        </b-col>
      </b-row>
      <b-row class="mt-4">
        <b-col class="mx-2 mx-lg-0">
          <b-alert variant="success" :show="contact.sent" class="parkpal-alert parkpal-alert--sent">
            <p>Your message has been sent successfully and we will aim to reply within 72 hours.</p>
          </b-alert>
          <b-alert variant="danger" :show="contact.errors.length > 0" class="parkpal-alert">
            <p>The form had the following errors, please fix them to send a message...</p>
            <ul>
              <li v-for="error in contact.errors" :key="error">{{ error }}</li>
            </ul>
          </b-alert>
        </b-col>
      </b-row>
      <div class="get-in-touch mx-2 mx-lg-0">
        <b-row>
          <b-col lg="7">
            <b-input type="text" placeholder="Name" class="parkpal-input mb-2" v-model="contact.form.name"></b-input>
            <b-input type="text" placeholder="Email Address" class="parkpal-input mb-2" v-model="contact.form.email"></b-input>
            <div class="parkpal-input mb-2 mb-lg-0">
              <b-select class="w-100 parkpal-input__select" v-model="contact.form.subject">
                <b-select-option value="Select a subject..." disabled>Select a subject...</b-select-option>
                <b-select-option value="Application Idea">Application Idea</b-select-option>
                <b-select-option value="Feature Request">Feature Request</b-select-option>
                <b-select-option value="General Feedback">General Feedback</b-select-option>
                <b-select-option value="Complaint">Complaint</b-select-option>
                <b-select-option value="Other">Other</b-select-option>
              </b-select>
            </div>
          </b-col>
          <b-col lg="5">
            <b-textarea placeholder="Message" class="h-100 mb-2 parkpal-input" v-model="contact.form.message"></b-textarea>
          </b-col>
        </b-row>
        <b-row class="mt-2">
          <b-col lg="6">
            <b-button type="button" class="w-100 parkpal-button parkpal-button--secondary mb-2 mb-lg-0" @click="clearForm">CLEAR FORM</b-button>
          </b-col>
          <b-col lg="6">
            <b-button type="button" class="w-100 parkpal-button parkpal-button--primary" @click="verifyRecaptcha">SEND MESSAGE</b-button>
          </b-col>
        </b-row>
        <b-row>
          <VueRecaptcha size="invisible" :loadRecaptchaScript="true" ref="invisibleRecaptcha" sitekey="6LfgGrcjAAAAAP3dHY7-V_rNkFUnxCtjzOEfXvQC" @verify="sendContactMessage"></VueRecaptcha>
        </b-row>
      </div>
    </b-container>

    <b-container class="content mt-5 content--footer" fluid>
      <footer class="footer">
        <b-row>
          <b-col>
            <img src="@/assets/logo.png" alt="ParkPal Logo">
          </b-col>
        </b-row>
        <b-row>
          <b-col cols="12" lg="8" offset-lg="2">
            <ul class="footer-navigation">
              <li class="footer-navigation__link">
                <b-nav-item href="#download">DOWNLOAD</b-nav-item>
              </li>
              <li class="footer-navigation__link">
                <b-nav-item href="#themeparks">THEME PARKS</b-nav-item>
              </li>
              <li class="footer-navigation__link">
                <b-nav-item href="#features">FEATURES</b-nav-item>
              </li>
              <li class="footer-navigation__link">
                <b-nav-item href="#screenshots">APPLICATION</b-nav-item>
              </li>
              <li class="footer-navigation__link">
                <b-nav-item href="#getintouch">GET IN TOUCH</b-nav-item>
              </li>
            </ul>
          </b-col>
        </b-row>
        <b-row>
          <b-col>
            <ul class="footer-socials">
              <li class="footer-socials__link">
                <a href="https://twitter.com/parkpaluk" target="_blank">
                  <font-awesome-icon :icon="['brands', 'twitter']" fixed-width></font-awesome-icon>
                </a>
              </li>
              <li class="footer-socials__link">
                <a href="https://www.instagram.com/parkpalapp/" target="_blank">
                  <font-awesome-icon :icon="['brands', 'instagram']" fixed-width></font-awesome-icon>
                </a>
              </li>
              <li class="footer-socials__link">
                <a href="https://www.facebook.com/profile.php?id=100088720824760" target="_blank">
                  <font-awesome-icon :icon="['brands', 'facebook']" fixed-width></font-awesome-icon>
                </a>
              </li>
            </ul>
          </b-col>
        </b-row>
      </footer>
    </b-container>

    <b-container class="content" fluid>
      <b-row>
        <b-col class="copyright">
          <div class="copyright__notice">
            <p>&copy; Website and Application by DevItUp Ltd</p>
          </div>
          <div class="copyright__devitup">
            <img src="@/assets/devitup-logo.png" border="0" width="50" alt="DevItUp Logo">
          </div>
        </b-col>
      </b-row>
    </b-container>

  </div>
</template>

<style lang="scss">
@import '@/assets/styles/app.scss';

#app {
  height: 100%;
  font-family: Avenir, Helvetica, Arial, sans-serif;
  text-align: center;
  color: #2c3e50;
  padding-top: 80px;
}
</style>

<script>
import { SwiperCore, Swiper, SwiperSlide } from 'swiper-vue2';
import { VueRecaptcha } from 'vue-recaptcha';
import axios from 'axios';

import { Navigation } from "swiper";

SwiperCore.use([Navigation])

export default {
  name: 'App',
  components: {
    Swiper,
    SwiperSlide,
    VueRecaptcha
  },
  data() {
    return {
      modules: [Navigation],
      contact: {
        loading: false,
        form: {
          name: "",
          email: "",
          subject: "Select a subject...",
          message: ""
        },
        errors: [],
        sent: false
      }
    }
  },
  methods: {
    contactFormValid() {
      this.contact.errors = [];

      if (this.contact.form.name.length < 3) {
        this.contact.errors.push('Name must have at least 3 characters.');
      }

      if (this.contact.form.email.length < 5) {
        this.contact.errors.push('Email must have at least 5 characters.');
      } else {
        // eslint-disable-next-line
        const res = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        if (!res.test(this.contact.form.email.toLowerCase())) {
          this.contact.errors.push('You must enter a valid email address.');
        }
      }

      if (this.contact.form.subject == "Select a subject...") {
        this.contact.errors.push('You must select a subject from the list provided.');
      }

      if (this.contact.form.message.length < 12) {
        this.contact.errors.push('Message must have at least 12 characters.');
      }

      return !this.contact.errors.length;
    },
    verifyRecaptcha() {
      this.$refs.invisibleRecaptcha.execute();
    },
    sendContactMessage() {
      this.contact.loading = true;

      if(this.contactFormValid()) {
        axios.defaults.headers.post['Content-Type'] = 'application/json';

        axios.post('https://formsubmit.co/ajax/7d17f4e933aa4969aff6bb4d0ce4b196', {
          _template: 'table',
          name: this.contact.form.name,
          email: this.contact.form.email,
          subject: this.contact.form.subject,
          message: this.contact.form.message
        })
        .then(response => {
          if (response.data.success) {
            this.clearForm();
            this.contact.sent = true;
          } else {
            this.contact.errors.push('There was an unexpected error sending your message, please try again.');
          }
        })
        .catch(error => {
          // Show a generic error here.
          console.log(error);
          this.contact.errors.push('There was an unexpected error sending your message, please try again.');
        }).finally(() => {
          this.contact.loading = false;
        });
      }else{
        this.$refs.invisibleRecaptcha.reset();
        this.contact.loading = false;
      }
    },
    clearForm() {
      this.$refs.invisibleRecaptcha.reset();

      this.contact.form = {
          name: "",
          email: "",
          subject: "Select a subject...",
          message: ""
      }

      this.contact.errors = [];
    }
  }
}
</script>
