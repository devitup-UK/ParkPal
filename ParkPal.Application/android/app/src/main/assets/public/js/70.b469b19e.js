"use strict";(self["webpackChunkparkpal_application"]=self["webpackChunkparkpal_application"]||[]).push([[70],{8990:function(t,e,n){n.r(e),n.d(e,{createSwipeBackGesture:function(){return c}});var r=n(2873),a=n(2562),i=n(8016);
/*!
 * (C) Ionic http://ionicframework.com - MIT License
 */
const c=(t,e,n,c,o)=>{const s=t.ownerDocument.defaultView,u=(0,a.i)(t),l=t=>{const e=50,{startX:n}=t;return u?n>=s.innerWidth-e:n<=e},p=t=>u?-t.deltaX:t.deltaX,h=t=>u?-t.velocityX:t.velocityX,d=t=>l(t)&&e(),k=t=>{const e=p(t),n=e/s.innerWidth;c(n)},f=t=>{const e=p(t),n=s.innerWidth,a=e/n,i=h(t),c=n/2,u=i>=0&&(i>.2||e>c),l=u?1-a:a,d=l*n;let k=0;if(d>5){const t=d/Math.abs(i);k=Math.min(t,540)}o(u,a<=0?.01:(0,r.h)(0,a,.9999),k)};return(0,i.createGesture)({el:t,gestureName:"goback-swipe",gesturePriority:40,threshold:10,canStart:d,onStart:n,onMove:k,onEnd:f})}}}]);
//# sourceMappingURL=70.b469b19e.js.map