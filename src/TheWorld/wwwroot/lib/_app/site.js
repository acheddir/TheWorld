!function(){function h(){var h=$(window).height(),t=$(".nav").height()+50;height2=$(".main").height(),height2>h?($("html").height(Math.max(t,h,height2)+10),$("body").height(Math.max(t,h,height2)+10)):($("html").height(Math.max(t,h,height2)),$("body").height(Math.max(t,h,height2)))}$(document).ready(function(){h(),$(window).resize(function(){h()}),$(window).scroll(function(){height2=$(".main").height(),h()})})}();