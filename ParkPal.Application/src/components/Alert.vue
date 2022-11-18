<template>
  <div class="parkpal-alert" :class="'parkpal-alert--' + mode">
    <p><slot></slot></p>
  </div>
</template>

<script>
export default {
  name: "AlertComponent",
  props: {
    mode: {
      type: String,
      default: 'primary'
    }
  }
}
</script>

<style lang="scss" scoped>
.parkpal-alert {
  font-size: 13px;
  font-weight: 400;
  color: #FFF;
  text-align: center;
  padding: 10px 18px;
  background: #BABABA;
  border-color: #707070;
  border-width: 2px 0;
  border-style: solid;
  margin-top: 10px;

  p {
    margin: 0;
  }

  &.parkpal-alert--warning {
    background: #FF4141;
    border-color: #a81d1d;
  }

  &.parkpal-alert--success {
    background: #64cc66;
    border-color: #5f8d3c;
  }
}
</style>