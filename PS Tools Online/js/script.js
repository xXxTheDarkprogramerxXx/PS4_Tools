"use strict";
(function () {

	/**
	 * Variables
	 */
	var userAgent = navigator.userAgent.toLowerCase(),
		initialDate = new Date(),

		$document = $(document),
		$window = $(window),
		$html = $("html"),

		isDesktop = $html.hasClass("desktop"),
		isRtl = $html.attr("dir") === "rtl",
		isIE = userAgent.indexOf("msie") != -1 ? parseInt(userAgent.split("msie")[1], 10) : userAgent.indexOf("trident") != -1 ? 11 : userAgent.indexOf("edge") != -1 ? 12 : false,
		isMobile = /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent),
		onloadCaptchaCallback,
		livedemo = false,
		isNoviBuilder = window.xMode,

		plugins = {
			pointerEvents: isIE < 11 ? "js/pointer-events.min.js" : false,
			bootstrapTooltip: $("[data-toggle='tooltip']"),
			bootstrapTabs: $(".tabs"),
			rdNavbar: $(".rd-navbar"),
			materialParallax: $(".parallax-container"),
			rdMailForm: $(".rd-mailform"),
			rdInputLabel: $(".form-label"),
			regula: $("[data-constraints]"),
			owl: $(".owl-carousel"),
			swiper: $(".swiper-container"),
			search: $(".rd-search"),
			searchResults: $('.rd-search-results'),
			statefulButton: $('.btn-stateful'),
			isotope: $(".isotope"),
			popover: $('[data-toggle="popover"]'),
			viewAnimate: $('.view-animate'),
			radio: $("input[type='radio']"),
			checkbox: $("input[type='checkbox']"),
			customToggle: $("[data-custom-toggle]"),
			counter: $(".counter"),
			selectFilter: $("select"),
			flickrfeed: $(".flickr"),
			captcha: $('.recaptcha'),
			scroller: $(".scroll-wrap"),
			pageLoader: $(".page-loader"),
			customWaypoints: $('[data-custom-scroll-to]'),
			stepper: $("input[type='number']"),
			customParallax: $(".custom-parallax"),
			vide: $(".vide_bg"),
			copyrightYear: $(".copyright-year"),
			mailchimp: $('.mailchimp-mailform'),
			campaignMonitor: $('.campaign-mailform'),
			jPlayer: $('.jp-jplayer'),
			jPlayerInit: $('.jp-player-init'),
			jPlayerVideo: $('.jp-video-init'),
			instafeed: $(".instafeed"),
			lightGallery: $("[data-lightgallery='group']"),
			lightGalleryItem: $("[data-lightgallery='item']"),
			lightDynamicGalleryItem: $("[data-lightgallery='dynamic']"),
			twitterfeed: $(".twitter"),
			facebookfeed: $(".facebook"),
			circleProgress: $(".progress-bar-circle"),
			countDown: $(".countdown"),
			svgCountDown: $('[data-circle-countdown]'),
			animtext: $('.textillate'),
			typedjs: $('.typed-text-wrap'),
			showMoreToggle: $('.slide-down-toggle'),
			animBox: $('.animated-box-wrap'),
			maps: $(".google-map-container"),
			modal: $('.modal'),
			particles: $('#particles-js'),
			parallaxTextBody: $('.parallax-section-inner')
		};

	/**
	 * Initialize All Scripts
	 */

	/**
	 * Page loader
	 * @description Enables Page loader
	 */
	if (plugins.pageLoader.length > 0 && !isNoviBuilder) {
		$window.on('load', function () {
			plugins.pageLoader.fadeOut('slow');
			$window.trigger("resize"); 
		});
	} else {
		plugins.pageLoader.remove();
	}
	;

	$(function () {

		/**
		 * getSwiperHeight
		 * @description  calculate the height of swiper slider basing on data attr
		 */
		function getSwiperHeight(object, attr) {
			var val = object.attr("data-" + attr),
				dim;

			if (!val) {
				return undefined;
			}

			dim = val.match(/(px)|(%)|(vh)$/i);

			if (dim.length) {
				switch (dim[0]) {
					case "px":
						return parseFloat(val);
					case "vh":
						return $window.height() * (parseFloat(val) / 100);
					case "%":
						return object.width() * (parseFloat(val) / 100);
				}
			} else {
				return undefined;
			}
		}

		/**
		 * Google map function for getting latitude and longitude
		 */
		function getLatLngObject(str, marker, map, callback) {
			var coordinates = {};
			try {
				coordinates = JSON.parse(str);
				callback(new google.maps.LatLng(
					coordinates.lat,
					coordinates.lng
				), marker, map)
			} catch (e) {
				map.geocoder.geocode({'address': str}, function (results, status) {
					if (status === google.maps.GeocoderStatus.OK) {
						var latitude = results[0].geometry.location.lat();
						var longitude = results[0].geometry.location.lng();

						callback(new google.maps.LatLng(
							parseFloat(latitude),
							parseFloat(longitude)
						), marker, map)
					}
				})
			}
		}

		/**
		 * function for animate text body on landing page
		 */
		function parallaxScroll() {
			offsetTop = window.pageYOffset;
			heightParLayer = inner.parentNode.clientHeight;
			innerHeight = inner.clientHeight;
			zInner = (offsetTop ) / heightParLayer * 200;
			o = ((innerHeight - (offsetTop * .7)) / innerHeight).toFixed(2);
			inner.style.setProperty('--to', (o));
			inner.style.setProperty('--tzInner', (zInner) + 'px');
		};

		/**
		 * toggleSwiperInnerVideos
		 * @description  toggle swiper videos on active slides
		 */
		function toggleSwiperInnerVideos(swiper) {
			var prevSlide = $(swiper.slides[swiper.previousIndex]),
				nextSlide = $(swiper.slides[swiper.activeIndex]),
				videos,
				videoItems = prevSlide.find("video");

			for (var i = 0; i < videoItems.length; i++) {
				videoItems[i].pause();
			}

			videos = nextSlide.find("video");
			if (!isNoviBuilder && videos.length) {
				videos.get(0).play();
				videos.css({'visibility': 'visible', 'opacity': '1'});
			}
		}

		/**
		 * toggleSwiperCaptionAnimation
		 * @description  toggle swiper animations on active slides
		 */
		function toggleSwiperCaptionAnimation(swiper) {
			var prevSlide = $(swiper.container).find("[data-caption-animate]"),
				nextSlide = $(swiper.slides[swiper.activeIndex]).find("[data-caption-animate]"),
				delay,
				duration,
				nextSlideItem,
				prevSlideItem;

			for (var i = 0; i < prevSlide.length; i++) {
				prevSlideItem = $(prevSlide[i]);

				prevSlideItem.removeClass("animated")
					.removeClass(prevSlideItem.attr("data-caption-animate"))
					.addClass("not-animated");
			}

			for (var i = 0; i < nextSlide.length; i++) {
				nextSlideItem = $(nextSlide[i]);
				delay = nextSlideItem.attr("data-caption-delay");
				duration = nextSlideItem.attr('data-caption-duration');

				var tempFunction = function (nextSlideItem, duration) {
					return function () {
						nextSlideItem
							.removeClass("not-animated")
							.addClass(nextSlideItem.attr("data-caption-animate"))
							.addClass("animated");

						if (duration) {
							nextSlideItem.css('animation-duration', duration + 'ms');
						}
					};
				};

				setTimeout(tempFunction(nextSlideItem, duration), delay ? parseInt(delay, 10) : 0);
			}
		}

		/**
		 * makeParallax
		 * @description  create swiper parallax scrolling effect
		 */
		function makeParallax(el, speed, wrapper, prevScroll) {
			var scrollY = window.scrollY || window.pageYOffset;

			if (prevScroll != scrollY) {
				prevScroll = scrollY;
				el.addClass('no-transition');
				el[0].style['transform'] = 'translate3d(0,' + -scrollY * (1 - speed) + 'px,0)';
				el.height();
				el.removeClass('no-transition');

				if (el.attr('data-fade') === 'true') {
					var bound = el[0].getBoundingClientRect(),
						offsetTop = bound.top * 2 + scrollY,
						sceneHeight = wrapper.outerHeight(),
						sceneDevider = wrapper.offset().top + sceneHeight / 2.0,
						layerDevider = offsetTop + el.outerHeight() / 2.0,
						pos = sceneHeight / 6.0,
						opacity;
					if (sceneDevider + pos > layerDevider && sceneDevider - pos < layerDevider) {
						el[0].style["opacity"] = 1;
					} else {
						if (sceneDevider - pos < layerDevider) {
							opacity = 1 + ((sceneDevider + pos - layerDevider) / sceneHeight / 3.0 * 5);
						} else {
							opacity = 1 - ((sceneDevider - pos - layerDevider) / sceneHeight / 3.0 * 5);
						}
						el[0].style["opacity"] = opacity < 0 ? 0 : opacity > 1 ? 1 : opacity.toFixed(2);
					}
				}
			}

			requestAnimationFrame(function () {
				makeParallax(el, speed, wrapper, prevScroll);
			});
		}

		/**
		 * isScrolledIntoView
		 * @description  check the element whas been scrolled into the view
		 */
		function isScrolledIntoView(elem) {
			if (!isNoviBuilder) {
				return ( elem.offset().top + elem.outerHeight() >= $window.scrollTop() ) && ( elem.offset().top <= $window.scrollTop() + $window.height() );
			}
			else {
				return true;
			}
		}

		/**
		 * initOnView
		 * @description  calls a function when element has been scrolled into the view
		 */
		function lazyInit(element, func) {
			var $win = jQuery(window);
			$win.on('load scroll', function () {
				if ((!element.hasClass('lazy-loaded') && (isScrolledIntoView(element)))) {
					func.call();
					element.addClass('lazy-loaded');
				}
			});
		}

		/**
		 * Custom Waypoints
		 */
		if (plugins.customWaypoints.length && !isNoviBuilder) {
			var i;
			for (i = 0; i < plugins.customWaypoints.length; i++) {
				var $this = $(plugins.customWaypoints[i]);

				$this.on('click', function (e) {
					e.preventDefault();
					$("body, html").stop().animate({
						scrollTop: $("#" + $(this).attr('data-custom-scroll-to')).offset().top
					}, 1000, function () {
						$window.trigger("resize");
					});
				});
			}
		}

		/**
		 * Live Search
		 * @description  create live search results
		 */
		function liveSearch(options) {
			options.live.removeClass('cleared').html();
			options.current++;
			options.spin.addClass('loading');

			$.get(handler, {
				s: decodeURI(options.term),
				liveSearch: options.element.attr('data-search-live'),
				dataType: "html",
				liveCount: options.liveCount,
				filter: options.filter,
				template: options.template
			}, function (data) {
				options.processed++;
				var live = options.live;
				if (options.processed == options.current && !live.hasClass('cleared')) {
					live.find('> #search-results').removeClass('active');
					live.html(data);
					setTimeout(function () {
						live.find('> #search-results').addClass('active');
					}, 50);
				}
				options.spin.parents('.rd-search').find('.input-group-addon').removeClass('loading');
			})
		}

		/**
		 * attachFormValidator
		 * @description  attach form validation to elements
		 */
		function attachFormValidator(elements) {
			for (var i = 0; i < elements.length; i++) {
				var o = $(elements[i]), v;
				o.addClass("form-control-has-validation").after("<span class='form-validation'></span>");
				v = o.parent().find(".form-validation");
				if (v.is(":last-child")) {
					o.addClass("form-control-last-child");
				}
			}

			elements
				.on('input change propertychange blur', function (e) {
					var $this = $(this), results;

					if (e.type != "blur") {
						if (!$this.parent().hasClass("has-error")) {
							return;
						}
					}

					if ($this.parents('.rd-mailform').hasClass('success')) {
						return;
					}

					if ((results = $this.regula('validate')).length) {
						for (i = 0; i < results.length; i++) {
							$this.siblings(".form-validation").text(results[i].message).parent().addClass("has-error")
						}
					} else {
						$this.siblings(".form-validation").text("").parent().removeClass("has-error")
					}
				})
				.regula('bind');
		}

		/**
		 * isValidated
		 * @description  check if all elemnts pass validation
		 */
		function isValidated(elements, captcha) {
			var results, errors = 0;

			if (elements.length) {
				for (j = 0; j < elements.length; j++) {

					var $input = $(elements[j]);
					if ((results = $input.regula('validate')).length) {
						for (k = 0; k < results.length; k++) {
							errors++;
							$input.siblings(".form-validation").text(results[k].message).parent().addClass("has-error");
						}
					} else {
						$input.siblings(".form-validation").text("").parent().removeClass("has-error")
					}
				}

				if (captcha) {
					if (captcha.length) {
						return validateReCaptcha(captcha) && errors == 0
					}
				}

				return errors == 0;
			}
			return true;
		}


		/**
		 * validateReCaptcha
		 * @description  validate google reCaptcha
		 */
		function validateReCaptcha(captcha) {
			var $captchaToken = captcha.find('.g-recaptcha-response').val();

			if ($captchaToken == '') {
				captcha
					.siblings('.form-validation')
					.html('Please, prove that you are not robot.')
					.addClass('active');
				captcha
					.closest('.form-group')
					.addClass('has-error');

				captcha.on('propertychange', function () {
					var $this = $(this),
						$captchaToken = $this.find('.g-recaptcha-response').val();

					if ($captchaToken != '') {
						$this
							.closest('.form-group')
							.removeClass('has-error');
						$this
							.siblings('.form-validation')
							.removeClass('active')
							.html('');
						$this.off('propertychange');
					}
				});

				return false;
			}

			return true;
		}


		/**
		 * onloadCaptchaCallback
		 * @description  init google reCaptcha
		 */
		window.onloadCaptchaCallback = function () {
			for (i = 0; i < plugins.captcha.length; i++) {
				var $capthcaItem = $(plugins.captcha[i]);

				grecaptcha.render(
					$capthcaItem.attr('id'),
					{
						sitekey: $capthcaItem.attr('data-sitekey'),
						size: $capthcaItem.attr('data-size') ? $capthcaItem.attr('data-size') : 'normal',
						theme: $capthcaItem.attr('data-theme') ? $capthcaItem.attr('data-theme') : 'light',
						callback: function (e) {
							$('.recaptcha').trigger('propertychange');
						}
					}
				);
				$capthcaItem.after("<span class='form-validation'></span>");
			}
		}

		/**
		 * Init Bootstrap tooltip
		 * @description  calls a function when need to init bootstrap tooltips
		 */
		function initBootstrapTooltip(tooltipPlacement) {
			if (window.innerWidth < 599) {
				plugins.bootstrapTooltip.tooltip('destroy');
				plugins.bootstrapTooltip.tooltip({
					placement: 'bottom'
				});
			} else {
				plugins.bootstrapTooltip.tooltip('destroy');
				plugins.bootstrapTooltip.tooltipPlacement;
				plugins.bootstrapTooltip.tooltip();
			}
		}

		/**
		 * Copyright Year
		 * @description  Evaluates correct copyright year
		 */

		if (plugins.copyrightYear.length) {
			plugins.copyrightYear.text(initialDate.getFullYear());
		}

		/**
		 * Is Mac os
		 * @description  add additional class on html if mac os.
		 */
		if (navigator.platform.match(/(Mac)/i)) $html.addClass("mac-os");

		/**
		 *  Vide - v0.5.1
		 *  @description jQuery plugin for video backgrounds
		 */
		if (plugins.vide.length) {
			for (var i = 0; i < plugins.vide.length; i++) {

				var $element = $(plugins.vide[i]),
					videObj = $element.data("vide").getVideoObject();

				if (isNoviBuilder || !isScrolledIntoView($element)) {
					videObj.pause();
				}

				document.addEventListener('scroll', function ($element, videObj) {
					return function () {
						if (!isNoviBuilder && (isScrolledIntoView($element) || videObj.pause())) videObj.play();
						else videObj.pause();
					}
				}($element, videObj));

			}
		}

		/**
		 * Bootstrap tabs
		 * @description Activate Bootstrap Tabs
		 */

		if (plugins.bootstrapTabs.length) {
			var i;
			for (i = 0; i < plugins.bootstrapTabs.length; i++) {
				var bootstrapTabsItem = $(plugins.bootstrapTabs[i]),
					isURLTabs = bootstrapTabsItem.attr('data-url-tabs') == 'true',
					currentHash = window.location.hash,
					tabsNav = bootstrapTabsItem.find('.tabs-nav');

				if (isURLTabs) {
					$('[data-content-to]:first-of-type').addClass('show');
				}

				bootstrapTabsItem.find('.nav-tabs').on('click', 'a', (function (isURLTabs, currentHash) {
					return function (event) {
						var currentLink = $(this).attr('href');

						event.preventDefault();
						$(this).tab('show');

						if (isURLTabs) {
							currentHash = currentLink;
							window.location.hash = currentHash;
						}

						var currentItem = $('[data-content-to].show');
						currentItem.removeClass('show');

						var newItem = $('[data-content-to = ' + currentHash + ']');
						newItem.addClass('show');
					};
				})(isURLTabs, currentHash));

				if (isURLTabs && currentHash) {
					bootstrapTabsItem.find("a[href$='" + currentHash + "']").first().trigger('click');

					setTimeout(function () {
						window.scrollTo(0, 0);
					}, 200);
				}

				tabsNav.on('click', 'a', function (bootstrapTabsItem) {
					return function (e) {
						if ($(this).attr('href').indexOf('#') == -1) {
							return;
						}

						e.preventDefault();
						e.stopPropagation();
						bootstrapTabsItem.find("a[href$='" + $(this).attr('href').split('#').pop() + "']").first().trigger('click');
					};
				}(bootstrapTabsItem));
			}
		}


		/**
		 * IE Polyfills
		 * @description  Adds some loosing functionality to IE browsers
		 */
		if (isIE) {
			if (isIE < 10) {
				$html.addClass("lt-ie-10");
			}

			if (isIE < 11) {
				if (plugins.pointerEvents) {
					$.getScript(plugins.pointerEvents)
						.done(function () {
							$html.addClass("ie-10");
							PointerEventsPolyfill.initialize({});
						});
				}
			}

			if (isIE === 11) {
				$("html").addClass("ie-11");
			}

			if (isIE === 12) {
				$("html").addClass("ie-edge");
			}
		}

		/**
		 * Bootstrap Tooltips
		 * @description Activate Bootstrap Tooltips
		 */
		if (plugins.bootstrapTooltip.length) {
			var tooltipPlacement = plugins.bootstrapTooltip.attr('data-placement');
			initBootstrapTooltip(tooltipPlacement);
			$window.on('resize orientationchange', function () {
				initBootstrapTooltip(tooltipPlacement);
			})
		}

		/**
		 * bootstrapModalDialog
		 * @description Stap vioeo in bootstrapModalDialog
		 */
		if (plugins.modal.length > 0) {
			var i = 0;

			for (i = 0; i < plugins.modal.length; i++) {
				var modalItem = $(plugins.modal[i]);

				modalItem.on('hidden.bs.modal', $.proxy(function () {
					var activeModal = $(this),
						rdVideoInside = activeModal.find('video'),
						youTubeVideoInside = activeModal.find('iframe');

					if (rdVideoInside.length) {
						rdVideoInside[0].pause();
					}

					if (youTubeVideoInside.length) {
						var videoUrl = youTubeVideoInside.attr('src');

						youTubeVideoInside
							.attr('src', '')
							.attr('src', videoUrl);
					}
				}, modalItem))
			}
		}

		/**
		 * Radio
		 * @description Add custom styling options for input[type="radio"]
		 */
		if (plugins.radio.length) {
			var i;
			for (i = 0; i < plugins.radio.length; i++) {
				var $this = $(plugins.radio[i]);
				$this.addClass("radio-custom").after("<span class='radio-custom-dummy'></span>")
			}
		}

		/**
		 * Checkbox
		 * @description Add custom styling options for input[type="checkbox"]
		 */
		if (plugins.checkbox.length) {
			var i;
			for (i = 0; i < plugins.checkbox.length; i++) {
				var $this = $(plugins.checkbox[i]);
				$this.addClass("checkbox-custom").after("<span class='checkbox-custom-dummy'></span>")
			}
		}

		/**
		 * Popovers
		 * @description Enables Popovers plugin
		 */
		if (plugins.popover.length) {
			if (window.innerWidth < 767) {
				plugins.popover.attr('data-placement', 'bottom');
				plugins.popover.popover();
			}
			else {
				plugins.popover.popover();
			}
		}

		/**
		 * Bootstrap Buttons
		 * @description  Enable Bootstrap Buttons plugin
		 */
		if (plugins.statefulButton.length) {
			$(plugins.statefulButton).on('click', function () {
				var statefulButtonLoading = $(this).button('loading');

				setTimeout(function () {
					statefulButtonLoading.button('reset')
				}, 2000);
			})
		}

		/**
		 * RD Navbar
		 * @description Enables RD Navbar plugin
		 */
		if (plugins.rdNavbar.length) {
			plugins.rdNavbar.RDNavbar({
				anchorNav: !isNoviBuilder,
				autoHeight: false,
				stickUpClone: (plugins.rdNavbar.attr("data-stick-up-clone") && !isNoviBuilder) ? plugins.rdNavbar.attr("data-stick-up-clone") === 'true' : false,
				responsive: {
					0: {
						stickUp: (!isNoviBuilder) ? plugins.rdNavbar.attr("data-stick-up") === 'true' : false
					},
					768: {
						stickUp: (!isNoviBuilder) ? plugins.rdNavbar.attr("data-sm-stick-up") === 'true' : false
					},
					992: {
						stickUp: (!isNoviBuilder) ? plugins.rdNavbar.attr("data-md-stick-up") === 'true' : false
					},
					1200: {
						stickUp: (!isNoviBuilder) ? plugins.rdNavbar.attr("data-lg-stick-up") === 'true' : false
					}
				},
				callbacks: {
					onStuck: function () {
						var navbarSearch = this.$element.find('.rd-search input');

						if (navbarSearch) {
							navbarSearch.val('').trigger('propertychange');
						}
					},
					onDropdownOver: function () {
						return !isNoviBuilder;
					},
					onUnstuck: function () {
						if (this.$clone === null)
							return;

						var navbarSearch = this.$clone.find('.rd-search input');

						if (navbarSearch) {
							navbarSearch.val('').trigger('propertychange');
							navbarSearch.trigger('blur');
						}
					}
				}
			});


			if (plugins.rdNavbar.attr("data-body-class")) {
				document.body.className += ' ' + plugins.rdNavbar.attr("data-body-class");
			}
		}

		/**
		 * ViewPort Universal
		 * @description Add class in viewport
		 */
		if (plugins.viewAnimate.length) {
			var i;
			for (i = 0; i < plugins.viewAnimate.length; i++) {
				var $view = $(plugins.viewAnimate[i]).not('.active');
				$document.on("scroll", $.proxy(function () {
					if (isScrolledIntoView(this)) {
						this.addClass("active");
					}
				}, $view))
					.trigger("scroll");
			}
		}

		/**
		 * Swiper 3.1.7
		 * @description  Enable Swiper Slider
		 */

		if (plugins.swiper.length) {
			for (var i = 0; i < plugins.swiper.length; i++) {
				var s = $(plugins.swiper[i]);
				var pag = s.find(".swiper-pagination"),
					next = s.find(".swiper-button-next"),
					prev = s.find(".swiper-button-prev"),
					bar = s.find(".swiper-scrollbar"),
					parallax = s.parents('.rd-parallax').length,
					swiperSlide = s.find(".swiper-slide"),
					autoplay = false;

				for (var j = 0; j < swiperSlide.length; j++) {
					var $this = $(swiperSlide[j]),
						url;

					if (url = $this.attr("data-slide-bg")) {
						$this.css({
							"background-image": "url(" + url + ")",
							"background-size": "cover"
						})
					}
				}

				swiperSlide.end()
					.find("[data-caption-animate]")
					.addClass("not-animated")
					.end();

				var swiperOptions = {
					autoplay: isNoviBuilder ? null : s.attr('data-autoplay') ? s.attr('data-autoplay') === "false" ? undefined : s.attr('data-autoplay') : 5000,
					direction: s.attr('data-direction') ? s.attr('data-direction') : "horizontal",
					effect: s.attr('data-slide-effect') ? s.attr('data-slide-effect') : "slide",
					speed: s.attr('data-slide-speed') ? s.attr('data-slide-speed') : 600,
					autoHeight: s.attr('data-auto-height') ? s.attr('data-auto-height') === "true" : false,
					keyboardControl: s.attr('data-keyboard') === "true",
					mousewheelControl: s.attr('data-mousewheel') === "true",
					mousewheelReleaseOnEdges: s.attr('data-mousewheel-release') === "true",
					nextButton: next.length ? next.get(0) : (s.attr('data-custom-next') ? $(s.attr('data-custom-next')) : null),
					prevButton: prev.length ? prev.get(0) : (s.attr('data-custom-prev') ? $(s.attr('data-custom-prev')) : null),
					pagination: pag.length ? pag.get(0) : null,
					paginationType: s.attr('data-pagination-type') ? s.attr('data-pagination-type') : 'bullets',
					paginationClickable: pag.length ? pag.attr("data-clickable") !== "false" : false,
					paginationBulletRender: pag.length ? pag.attr("data-index-bullet") === "true" ? function (index, className) {
								return '<span class="' + className + '">' + (index + 1) + '</span>';
							} : null : null,
					scrollbar: bar.length ? bar.get(0) : null,
					scrollbarDraggable: bar.length ? bar.attr("data-draggable") !== "false" : true,
					scrollbarHide: bar.length ? bar.attr("data-draggable") === "false" : false,
					loop: isNoviBuilder ? false : s.attr('data-loop') !== "false",
					setWrapperSize: true,
					simulateTouch: s.attr('data-simulate-touch') && !isNoviBuilder ? s.attr('data-simulate-touch') === "true" : false,
					onTransitionStart: function (swiper) {
						toggleSwiperInnerVideos(swiper);
					},
					onTransitionEnd: function (swiper) {
						toggleSwiperCaptionAnimation(swiper);
					},
					onInit: function (swiper) {
						toggleSwiperInnerVideos(swiper);
						toggleSwiperCaptionAnimation(swiper);

						$window.on('resize', function () {
							swiper.update(true);
						})

						initLightGalleryItem(s.find('[data-lightgallery="item"]'), 'lightGallery-in-carousel');
					}
				};

				plugins.swiper[i] = s.swiper(swiperOptions);

				$window
					.on("resize", function () {
						var mh = getSwiperHeight(s, "min-height"), 
							h = getSwiperHeight(s, "height");
						if (h) {
							s.css("height", mh ? mh > h ? mh : h : h);
						}
					})
					.trigger("resize");

			}
		}

		/**
		 * Select2
		 * @description Enables select2 plugin
		 */
		if (plugins.selectFilter.length) {
			var i;
			for (i = 0; i < plugins.selectFilter.length; i++) {
				var select = $(plugins.selectFilter[i]);

				select.select2({
					theme: "bootstrap"
				}).next().addClass(select.attr("class").match(/(input-sm)|(input-lg)|($)/i).toString().replace(new RegExp(",", 'g'), " "));
			}
		}

		/**
		 * RD Search
		 * @description Enables search
		 */
		if (plugins.search.length || plugins.searchResults) {
			var handler = "bat/rd-search.php";
			var defaultTemplate = '<h5 class="search_title"><a target="_top" href="#{href}" class="search_link">#{title}</a></h5>' +
				'<p>...#{token}...</p>' +
				'<p class="match"><em>Terms matched: #{count} - URL: #{href}</em></p>';
			var defaultFilter = '*.html';

			if (plugins.search.length) {

				plugins.search = $('.' + plugins.search[0].className);

				for (i = 0; i < plugins.search.length; i++) {
					var searchItem = $(plugins.search[i]),
						options = {
							element: searchItem,
							filter: (searchItem.attr('data-search-filter')) ? searchItem.attr('data-search-filter') : defaultFilter,
							template: (searchItem.attr('data-search-template')) ? searchItem.attr('data-search-template') : defaultTemplate,
							live: (searchItem.attr('data-search-live')) ? (searchItem.find('.' + searchItem.attr('data-search-live'))) : false,
							liveCount: (searchItem.attr('data-search-live-count')) ? parseInt(searchItem.attr('data-search-live'), 10) : 4,
							current: 0, processed: 0, timer: {}
						};

					if ($('.rd-navbar-search-toggle').length) {
						var toggle = $('.rd-navbar-search-toggle');
						toggle.on('click', function () {
							if (!($(this).hasClass('active'))) {
								searchItem.find('input').val('').trigger('propertychange');
							}
						});
					}

					if (options.live) {
						options.clearHandler = false;

						searchItem.find('input').on("keyup input propertychange", $.proxy(function () {
							var ctx = this;

							this.term = this.element.find('input').val().trim();
							this.spin = this.element.find('.input-group-addon');

							clearTimeout(ctx.timer);

							if (ctx.term.length > 2) {
								ctx.timer = setTimeout(liveSearch(ctx), 200);

								if (ctx.clearHandler == false) {
									ctx.clearHandler = true;

									$("body").on("click", function (e) {
										if ($(e.toElement).parents('.rd-search').length == 0) {
											ctx.live.addClass('cleared').html('');
										}
									})
								}

							} else if (ctx.term.length == 0) {
								ctx.live.addClass('cleared').html('');
							}
						}, options, this));
					}

					searchItem.on('submit', $.proxy(function () {
						$('<input />').attr('type', 'hidden')
							.attr('name', "filter")
							.attr('value', this.filter)
							.appendTo(this.element);
						return true;
					}, options, this))
				}
			}

			if (plugins.searchResults.length) {
				var regExp = /\?.*s=([^&]+)\&filter=([^&]+)/g;
				var match = regExp.exec(location.search);

				if (match != null) {
					$.get(handler, {
						s: decodeURI(match[1]),
						dataType: "html",
						filter: match[2],
						template: defaultTemplate,
						live: ''
					}, function (data) {
						plugins.searchResults.html(data);
					})
				}
			}
		}

		/**
		 * Owl carousel
		 * @description Enables Owl carousel plugin
		 */
		if (plugins.owl.length) {
			var i;
			for (i = 0; i < plugins.owl.length; i++) {
				var c = $(plugins.owl[i]),
					responsive = {};
				plugins.owl[i].owl = c;

				var aliaces = ["-", "-xs-", "-sm-", "-md-", "-lg-", "-xl-"],
					values = [0, 480, 768, 992, 1200, 1800],
					j, k;

				for (j = 0; j < values.length; j++) {
					responsive[values[j]] = {};
					for (k = j; k >= -1; k--) {
						if (!responsive[values[j]]["items"] && c.attr("data" + aliaces[k] + "items")) {
							responsive[values[j]]["items"] = k < 0 ? 1 : parseInt(c.attr("data" + aliaces[k] + "items"), 10);
						}
						if (!responsive[values[j]]["stagePadding"] && responsive[values[j]]["stagePadding"] !== 0 && c.attr("data" + aliaces[k] + "stage-padding")) {
							responsive[values[j]]["stagePadding"] = k < 0 ? 0 : parseInt(c.attr("data" + aliaces[k] + "stage-padding"), 10);
						}
						if (!responsive[values[j]]["margin"] && responsive[values[j]]["margin"] !== 0 && c.attr("data" + aliaces[k] + "margin")) {
							responsive[values[j]]["margin"] = k < 0 ? 30 : parseInt(c.attr("data" + aliaces[k] + "margin"), 10);
						}
					}
				}

				// Create custom Numbering
				if (typeof(c.attr("data-numbering")) !== 'undefined') {
					var numberingObject = $(c.attr("data-numbering"));

					c.on('initialized.owl.carousel changed.owl.carousel', function (numberingObject) {
						return function (e) {
							if (!e.namespace) return;
							numberingObject.find('.numbering-current').text((e.item.index + 1) % e.item.count + 1);
							numberingObject.find('.numbering-count').text(e.item.count);
						};
					}(numberingObject));
				}

				// Enable custom pagination
				if (c.attr('data-dots-custom')) {
					c.on("initialized.owl.carousel", function (event) {
						var carousel = $(event.currentTarget),
							customPag = $(carousel.attr("data-dots-custom")),
							active = 0;

						if (carousel.attr('data-active')) {
							active = parseInt(carousel.attr('data-active'), 10);
						}

						carousel.trigger('to.owl.carousel', [active, 300, true]);
						customPag.find("[data-owl-item='" + active + "']").addClass("active");

						customPag.find("[data-owl-item]").on('click', function (e) {
							e.preventDefault();
							carousel.trigger('to.owl.carousel', [parseInt(this.getAttribute("data-owl-item"), 10), 300, true]);
						});

						carousel.on("translate.owl.carousel", function (event) {
							customPag.find(".active").removeClass("active");
							customPag.find("[data-owl-item='" + event.item.index + "']").addClass("active")
						});
					});
				}

				c.on("initialized.owl.carousel", function () {
					initLightGalleryItem(c.find('[data-lightgallery="item"]'), 'lightGallery-in-carousel');
				});

				c.owlCarousel({
					autoplay: isNoviBuilder ? false : c.attr("data-autoplay") === "true",
					loop: isNoviBuilder ? false : c.attr("data-loop") !== "false",
					items: 1,
					rtl: isRtl,
					dotsContainer: c.attr("data-pagination-class") || false,
					navContainer: c.attr("data-navigation-class") || false,
					mouseDrag: isNoviBuilder ? false : c.attr("data-mouse-drag") !== "false",
					nav: c.attr("data-nav") === "true",
					dots: ( isNoviBuilder && c.attr("data-nav") !== "true" )
						? true
						: c.attr("data-dots") === "true",
					dotsEach: c.attr("data-dots-each") ? parseInt(c.attr("data-dots-each"), 10) : false,
					animateIn: c.attr('data-animation-in') ? c.attr('data-animation-in') : false,
					animateOut: c.attr('data-animation-out') ? c.attr('data-animation-out') : false,
					responsive: responsive,
					center: c.attr("data-center") === "true",
					navText: function () {
						try {
							return JSON.parse(c.attr("data-nav-text"));
						} catch (e) {
							return [];
						}
					}(),
					navClass: function () {
						try {
							return JSON.parse(c.attr("data-nav-class"));
						} catch (e) {
							return ['owl-prev', 'owl-next'];
						}
					}()
				});
			}
		}

		/**
		 * WOW
		 * @description Enables Wow animation plugin
		 */
		if (!isNoviBuilder && isDesktop && $html.hasClass("wow-animation") && $(".wow").length) {
			new WOW().init();
		}

		/**
		 * RD Input Label
		 * @description Enables RD Input Label Plugin
		 */
		if (plugins.rdInputLabel.length) {
			plugins.rdInputLabel.RDInputLabel();
		}

		/**
		 * Regula
		 * @description Enables Regula plugin
		 */
		if (plugins.regula.length) {
			attachFormValidator(plugins.regula);
		}

		// MailChimp Ajax subscription
		if (plugins.mailchimp.length) {
			for (i = 0; i < plugins.mailchimp.length; i++) {
				var $mailchimpItem = $(plugins.mailchimp[i]),
					$email = $mailchimpItem.find('input[type="email"]');

				// Required by MailChimp
				$mailchimpItem.attr('novalidate', 'true');
				$email.attr('name', 'EMAIL');

				$mailchimpItem.on('submit', $.proxy( function ( $email, event ) {
					event.preventDefault();

					var $this = this;

					var data = {},
						url = $this.attr('action').replace('/post?', '/post-json?').concat('&c=?'),
						dataArray = $this.serializeArray(),
						$output = $("#" + $this.attr("data-form-output"));

					for (i = 0; i < dataArray.length; i++) {
						data[dataArray[i].name] = dataArray[i].value;
					}

					$.ajax({
						data: data,
						url: url,
						dataType: 'jsonp',
						error: function (resp, text) {
							$output.html('Server error: ' + text);

							setTimeout(function () {
								$output.removeClass("active");
							}, 4000);
						},
						success: function (resp) {
							$output.html(resp.msg).addClass('active');
							$email[0].value = '';
							var $label = $('[for="'+ $email.attr( 'id' ) +'"]');
							if ( $label.length ) $label.removeClass( 'focus not-empty' );

							setTimeout(function () {
								$output.removeClass("active");
							}, 6000);
						},
						beforeSend: function (data) {
							var isNoviBuilder = window.xMode;

							var isValidated = (function () {
								var results, errors = 0;
								var elements = $this.find('[data-constraints]');
								var captcha = null;
								if (elements.length) {
									for (var j = 0; j < elements.length; j++) {

										var $input = $(elements[j]);
										if ((results = $input.regula('validate')).length) {
											for (var k = 0; k < results.length; k++) {
												errors++;
												$input.siblings(".form-validation").text(results[k].message).parent().addClass("has-error");
											}
										} else {
											$input.siblings(".form-validation").text("").parent().removeClass("has-error")
										}
									}

									if (captcha) {
										if (captcha.length) {
											return validateReCaptcha(captcha) && errors === 0
										}
									}

									return errors === 0;
								}
								return true;
							})();

							// Stop request if builder or inputs are invalide
							if (isNoviBuilder || !isValidated)
								return false;

							$output.html('Submitting...').addClass('active');
						}
					});

					return false;
				}, $mailchimpItem, $email ));
			}
		}

		// Campaign Monitor ajax subscription
		if (plugins.campaignMonitor.length) {
			for (i = 0; i < plugins.campaignMonitor.length; i++) {
				var $campaignItem = $(plugins.campaignMonitor[i]);

				$campaignItem.on('submit', $.proxy(function (e) {
					var data = {},
						url = this.attr('action'),
						dataArray = this.serializeArray(),
						$output = $("#" + plugins.campaignMonitor.attr("data-form-output")),
						$this = $(this);

					for (i = 0; i < dataArray.length; i++) {
						data[dataArray[i].name] = dataArray[i].value;
					}

					$.ajax({
						data: data,
						url: url,
						dataType: 'jsonp',
						error: function (resp, text) {
							$output.html('Server error: ' + text);

							setTimeout(function () {
								$output.removeClass("active");
							}, 4000);
						},
						success: function (resp) {
							$output.html(resp.Message).addClass('active');

							setTimeout(function () {
								$output.removeClass("active");
							}, 6000);
						},
						beforeSend: function (data) {
							// Stop request if builder or inputs are invalide
							if (isNoviBuilder || !isValidated($this.find('[data-constraints]')))
								return false;

							$output.html('Submitting...').addClass('active');
						}
					});

					// Clear inputs after submit
					var inputs = $this[0].getElementsByTagName('input');
					for (var i = 0; i < inputs.length; i++) {
						inputs[i].value = '';
						var label = document.querySelector( '[for="'+ inputs[i].getAttribute( 'id' ) +'"]' );
						if( label ) label.classList.remove( 'focus', 'not-empty' );
					}

					return false;
				}, $campaignItem));
			}
		}

		/**
		 * Google ReCaptcha
		 * @description Enables Google ReCaptcha
		 */
		if (plugins.captcha.length) {
			var i;
			$.getScript("//www.google.com/recaptcha/api.js?onload=onloadCaptchaCallback&render=explicit&hl=en");
		}

		/**
		 * RD Mailform
		 * @version      3.2.0
		 */
		if (plugins.rdMailForm.length) {
			var i, j, k,
				msg = {
					'MF000': 'Successfully sent!',
					'MF001': 'Recipients are not set!',
					'MF002': 'Form will not work locally!',
					'MF003': 'Please, define email field in your form!',
					'MF004': 'Please, define type of your form!',
					'MF254': 'Something went wrong with PHPMailer!',
					'MF255': 'Aw, snap! Something went wrong.'
				};

			for (i = 0; i < plugins.rdMailForm.length; i++) {
				var $form = $(plugins.rdMailForm[i]),
					formHasCaptcha = false;

				$form.attr('novalidate', 'novalidate').ajaxForm({
					data: {
						"form-type": $form.attr("data-form-type") || "contact",
						"counter": i
					},
					beforeSubmit: function (arr, $form, options) {
						if (isNoviBuilder)
							return;

						var form = $(plugins.rdMailForm[this.extraData.counter]),
							inputs = form.find("[data-constraints]"),
							output = $("#" + form.attr("data-form-output")),
							captcha = form.find('.recaptcha'),
							captchaFlag = true;

						output.removeClass("active error success");

						if (isValidated(inputs, captcha)) {

							// veify reCaptcha
							if (captcha.length) {
								var captchaToken = captcha.find('.g-recaptcha-response').val(),
									captchaMsg = {
										'CPT001': 'Please, setup you "site key" and "secret key" of reCaptcha',
										'CPT002': 'Something wrong with google reCaptcha'
									};

								formHasCaptcha = true;

								$.ajax({
									method: "POST",
									url: "bat/reCaptcha.php",
									data: {'g-recaptcha-response': captchaToken},
									async: false
								})
									.done(function (responceCode) {
										if (responceCode !== 'CPT000') {
											if (output.hasClass("snackbars")) {
												output.html('<p><span class="icon text-middle mdi mdi-check icon-xxs"></span><span>' + captchaMsg[responceCode] + '</span></p>')

												setTimeout(function () {
													output.removeClass("active");
												}, 3500);

												captchaFlag = false;
											} else {
												output.html(captchaMsg[responceCode]);
											}

											output.addClass("active");
										}
									});
							}

							if (!captchaFlag) {
								return false;
							}

							form.addClass('form-in-process');

							if (output.hasClass("snackbars")) {
								output.html('<p><span class="icon text-middle fa fa-circle-o-notch fa-spin icon-xxs"></span><span>Sending</span></p>');
								output.addClass("active");
							}
						} else {
							return false;
						}
					},
					error: function (result) {
						if (isNoviBuilder)
							return;

						var output = $("#" + $(plugins.rdMailForm[this.extraData.counter]).attr("data-form-output")),
							form = $(plugins.rdMailForm[this.extraData.counter]);

						output.text(msg[result]);
						form.removeClass('form-in-process');

						if (formHasCaptcha) {
							grecaptcha.reset();
						}
					},
					success: function (result) {
						if (isNoviBuilder)
							return;

						var form = $(plugins.rdMailForm[this.extraData.counter]),
							output = $("#" + form.attr("data-form-output")),
							select = form.find('select');

						form
							.addClass('success')
							.removeClass('form-in-process');

						if (formHasCaptcha) {
							grecaptcha.reset();
						}

						result = result.length === 5 ? result : 'MF255';
						output.text(msg[result]);

						if (result === "MF000") {
							if (output.hasClass("snackbars")) {
								output.html('<p><span class="icon text-middle mdi mdi-check icon-xxs"></span><span>' + msg[result] + '</span></p>');
							} else {
								output.addClass("active success");
							}
						} else {
							if (output.hasClass("snackbars")) {
								output.html(' <p class="snackbars-left"><span class="icon icon-xxs mdi mdi-alert-outline text-middle"></span><span>' + msg[result] + '</span></p>');
							} else {
								output.addClass("active error");
							}
						}

						form.clearForm();

						if (select.length) {
							select.select2("val", "");
						}

						form.find('input, textarea').trigger('blur');

						setTimeout(function () {
							output.removeClass("active error success");
							form.removeClass('success');
						}, 3500);
					}
				});
			}
		}

		/**
		 * Custom Toggles
		 */
		if (plugins.customToggle.length) {
			var i;

			for (i = 0; i < plugins.customToggle.length; i++) {
				var $this = $(plugins.customToggle[i]);

				$this.on('click', $.proxy(function (event) {
					event.preventDefault();
					var $ctx = $(this);
					$($ctx.attr('data-custom-toggle')).add(this).toggleClass('active');
				}, $this));

				if ($this.attr("data-custom-toggle-hide-on-blur") === "true") {
					$("body").on("click", $this, function (e) {
						if (e.target !== e.data[0]
							&& $(e.data.attr('data-custom-toggle')).find($(e.target)).length
							&& e.data.find($(e.target)).length == 0) {
							$(e.data.attr('data-custom-toggle')).add(e.data[0]).removeClass('active');
						}
					})
				}

				if ($this.attr("data-custom-toggle-disable-on-blur") === "true") {
					$("body").on("click", $this, function (e) {
						if (e.target !== e.data[0] && $(e.data.attr('data-custom-toggle')).find($(e.target)).length == 0 && e.data.find($(e.target)).length == 0) {
							$(e.data.attr('data-custom-toggle')).add(e.data[0]).removeClass('active');
						}
					})
				}
			}
		}

		/**
		 * jQuery Count To
		 * @description Enables Count To plugin
		 */
		if (plugins.counter.length) {
			var i;

			for (i = 0; i < plugins.counter.length; i++) {
				var $counterNotAnimated = $(plugins.counter[i]).not('.animated');
				$document
					.on("scroll", $.proxy(function () {
						var $this = this;

						if ((!$this.hasClass("animated")) && (isScrolledIntoView($this))) {
							$this.countTo({
								refreshInterval: 40,
								from: 0,
								to: parseInt($this.text(), 10),
								speed: $this.attr("data-speed") || 1000,
								formatter: function (value, options) {
									if ($this.attr('data-formatter') != 'false') {
										value = value.toFixed(options.decimals);
										if (value < 10) {
											return '0' + value;
										}
										return value;

									} else if (value.toString().indexOf('.') !== -1) {
										var decimals = $this.attr('data-to').split('.')[1];
										return value.toFixed(options.decimals) + '.' + decimals;

									} else {
										return value.toFixed(options.decimals);
									}

								}
							});
							$this.addClass('animated');
						}
					}, $counterNotAnimated))
					.trigger("scroll");
			}
		}

		/**
		 * RD Flickr Feed
		 * @description Enables RD Flickr Feed plugin
		 */
		if (plugins.flickrfeed.length > 0) {
			for (var i = 0; i < plugins.flickrfeed.length; i++) {
				var $flickrfeedItem = $(plugins.flickrfeed[i]);
				$flickrfeedItem.RDFlickr({
					callback: function ($flickrfeedItem) {
						return function () {
							var items = $flickrfeedItem.find("[data-lightgallery]");

							if (items.length) {
								for (var j = 0; j < items.length; j++) {
									var image = new Image();
									image.setAttribute('data-index', j);
									image.onload = function () {
										items[this.getAttribute('data-index')].setAttribute('data-size', this.naturalWidth + 'x' + this.naturalHeight);
									};
									image.src = items[j].getAttribute('href');
								}
							}
						}
					}($flickrfeedItem)
				});
			}
		}

		/**
		 * JQuery mousewheel plugin
		 * @description  Enables jquery mousewheel plugin
		 */
		if (plugins.scroller.length) {
			var i;
			for (i = 0; i < plugins.scroller.length; i++) {
				var scrollerItem = $(plugins.scroller[i]);

				scrollerItem.mCustomScrollbar({
					theme: scrollerItem.attr('data-theme') ? scrollerItem.attr('data-theme') : 'minimal',
					scrollInertia: 100,
					scrollButtons: {enable: false}
				});
			}
		}

		/**
		 * Isotope
		 * @description Enables Isotope plugin
		 */
		if (plugins.isotope.length) {
			var i, j, isogroup = [];
			for (i = 0; i < plugins.isotope.length; i++) {
				var isotopeItem = plugins.isotope[i],
					filterItems = $(isotopeItem).parents('.isotope-wrap').find('[data-isotope-filter]'),
					iso;

				iso = new Isotope(isotopeItem, {
					itemSelector: '.isotope-item',
					layoutMode: isotopeItem.getAttribute('data-isotope-layout') ? isotopeItem.getAttribute('data-isotope-layout') : 'masonry',
					filter: '*',
					masonry: {
						columnWidth: '.grid-size'
					}
				});

				isogroup.push(iso);

				filterItems.on("click", function (e) {
					e.preventDefault();
					var filter = $(this),
						iso = $('.isotope[data-isotope-group="' + this.getAttribute("data-isotope-group") + '"]'),
						filtersContainer = filter.closest(".isotope-filters");

					filtersContainer
						.find('.active')
						.removeClass("active");
					filter.addClass("active");

					iso.isotope({
						itemSelector: '.isotope-item',
						layoutMode: iso.attr('data-isotope-layout') ? iso.attr('data-isotope-layout') : 'masonry',
						filter: this.getAttribute("data-isotope-filter") == '*' ? '*' : '[data-filter*="' + this.getAttribute("data-isotope-filter") + '"]',
						percentPosition: true,
						masonry: {
							columnWidth: '.grid-size'
						}
					});

					$window.trigger('resize');

				}).eq(0).trigger("click");
			}

			$window.on('load', function () {
				setTimeout(function () {
					var i;
					for (i = 0; i < isogroup.length; i++) {
						isogroup[i].element.className += " isotope--loaded";
						isogroup[i].layout();
					}
				}, 600);

				setTimeout(function () {
					$window.trigger('resize');
				}, 800);
			});
		}


		/**
		 * Stepper
		 * @description Enables Stepper Plugin
		 */
		if (plugins.stepper.length) {
			plugins.stepper.stepper({
				labels: {
					up: "",
					down: ""
				}
			});
		}

		/**
		 * Material Parallax
		 * @description Enables Material Parallax plugin
		 */
		if (plugins.materialParallax.length) {
			var i;

			if (!isNoviBuilder && !isIE && !isMobile) {
				plugins.materialParallax.parallax();
			} else {
				for (i = 0; i < plugins.materialParallax.length; i++) {
					var parallax = $(plugins.materialParallax[i]),
						imgPath = parallax.data("parallax-img");

					parallax.css({
						"background-image": 'url(' + imgPath + ')',
						"background-size": "cover"
					});
				}
			}
		}

		/**
		 * jpFormatePlaylistObj
		 * @description  format dynamic playlist object for jPlayer init
		 */
		function jpFormatePlaylistObj(playlistHtml) {
			var playlistObj = [];

			// Format object with audio
			for (var i = 0; i < playlistHtml.length; i++) {
				var playlistItem = playlistHtml[i],
					itemData = $(playlistItem).data();
				playlistObj[i] = {};

				for (var key in itemData) {
					playlistObj[i][key.replace('jp', '').toLowerCase()] = itemData[key];
				}
			}

			return playlistObj;
		}

		/**
		 * initJplayerBase
		 * @description Base jPlayer init
		 */
		function initJplayerBase(index, item, mediaObj) {
			return new jPlayerPlaylist({
				jPlayer: item.getElementsByClassName("jp-jplayer")[0],
				cssSelectorAncestor: ".jp-audio-" + index // Need too bee a selector not HTMLElement or Jq object, so we make it unique
			}, mediaObj, {
				playlistOptions: {
					enableRemoveControls: false
				},
				supplied: "ogv, m4v, oga, mp3",
				useStateClassSkin: true,
				volume: 0.4
			});
		}


		/**
		 * Jp Audio player
		 * @description  Custom jPlayer script
		 */

		if (plugins.jPlayerInit.length) {
			$html.addClass('ontouchstart' in window || 'onmsgesturechange' in window ? 'touch' : 'no-touch');

			$.each(plugins.jPlayerInit, function (index, item) {
				$(item).addClass('jp-audio-' + index);

				var mediaObj = jpFormatePlaylistObj($(item).find('.jp-player-list .jp-player-list-item')),
					playerInstance = initJplayerBase(index, item, mediaObj);

				if ($(item).data('jp-player-name')) {
					var customJpPlaylists = $('[data-jp-playlist-relative-to="' + $(item).data('jp-player-name') + '"]'),
						playlistItems = customJpPlaylists.find("[data-jp-playlist-item]");

					// Toggle audio play on custom playlist play button click
					playlistItems.on('click', customJpPlaylists.data('jp-playlist-play-on'), function (e) {
						var mediaObj = jpFormatePlaylistObj(playlistItems),
							$clickedItem = $(e.delegateTarget);

						if (!JSON.stringify(playerInstance.playlist) === JSON.stringify(mediaObj) || !playerInstance.playlist.length) {
							playerInstance.setPlaylist(mediaObj);
						}

						if (!$clickedItem.hasClass('playing')) {
							playerInstance.pause();

							if ($clickedItem.hasClass('last-played')) {
								playerInstance.play();
							} else {
								playerInstance.play(playlistItems.index($clickedItem));
							}

							playlistItems.removeClass('playing last-played');
							$clickedItem.addClass('playing');
						} else {
							playlistItems.removeClass('playing last-played');
							$clickedItem.addClass('last-played');
							playerInstance.pause();
						}

					});


					// Callback for custom playlist
					$(playerInstance.cssSelector.jPlayer).bind($.jPlayer.event.play, function (e) {

						var toggleState = function (elemClass, index) {
							var activeIndex = playlistItems.index(playlistItems.filter(elemClass));

							if (activeIndex !== -1) {
								if (playlistItems.eq(activeIndex + index).length !== 0) {
									playlistItems.eq(activeIndex)
										.removeClass('play-next play-prev playing last-played')
										.end()
										.eq(activeIndex + index)
										.addClass('playing');
								}
							}
						};

						// Check if user select next or prev track
						toggleState('.play-next', +1);
						toggleState('.play-prev', -1);

						var lastPlayed = playlistItems.filter('.last-played');

						// If user just press pause and than play on same track
						if (lastPlayed.length) {
							lastPlayed.addClass('playing').removeClass('last-played play-next');
						}
					});


					// Add temp marker of last played audio
					$(playerInstance.cssSelector.jPlayer).bind($.jPlayer.event.pause, function (e) {
						playlistItems.filter('.playing').addClass('last-played').removeClass('playing');

						$(playerInstance.cssSelector.cssSelectorAncestor).addClass('jp-state-visible');
					});

					// Add temp marker that user want to play next audio
					$(item).find('.jp-next')
						.on('click', function (e) {
							playlistItems.filter('.playing, .last-played').addClass('play-next');
						});

					// Add temp marker that user want to play prev audio
					$(item).find('.jp-previous')
						.on('click', function (e) {
							playlistItems.filter('.playing, .last-played').addClass('play-prev');
						});
				}
			});

		}

		/**
		 * Jp Video player
		 * @description  Custom jPlayer video initialization
		 */

		if (plugins.jPlayerVideo.length) {
			$.each(plugins.jPlayerVideo, function (index, item) {
				var $item = $(item);

				$item.find('.jp-video').addClass('jp-video-' + index);

				new jPlayerPlaylist({
					jPlayer: item.getElementsByClassName("jp-jplayer")[0],
					cssSelectorAncestor: ".jp-video-" + index // Need too bee a selector not HTMLElement or Jq object, so we make it unique
				}, jpFormatePlaylistObj($(item).find('.jp-player-list .jp-player-list-item')), {
					playlistOptions: {
						enableRemoveControls: false
					},
					size: {
						width: "100%",
						height: "auto",
					},
					supplied: "webmv, ogv, m4v",
					useStateClassSkin: true,
					volume: 0.4
				});

				$(item).find(".jp-jplayer").on('click', function (e) {
					var $this = $(this);
					if ($('.jp-video-' + index).hasClass('jp-state-playing')) {
						$this.jPlayer("pause");
					} else {
						$this.jPlayer("play");
					}
				});

				var initialContainerWidth = $item.width();
				// this is the overall page container, so whatever is relevant to your page

				$window.resize(function () {
					if ($item.width() !== initialContainerWidth) {
						// checks current container size against it's rendered size on every resize.
						initialContainerWidth = $item.width();
						$item.trigger('resize', $item);
						//pass off to resize listener for performance
					}
				});
			});

			$window.on('resize', function (e) {
				$('.jp-video').each(function (index) {
					// find every instance of jplayer using a class in their default markup
					var $parentContainer = $(this).closest('.jp-video-init'),
						// finds jplayers closest parent element from the ones you give it (can chain as many as you want)
						containerWidth = $parentContainer.width(),
						//takes the closest elements width
						ARWidth = 1280,
						ARHeight = 720;

					// Width and height figures used to calculate the aspect ratio (will not restrict your players to this size)

					var aspectRatio = ARHeight / ARWidth;

					var videoHeight = Math.round(aspectRatio * containerWidth);
					// calculates the appropriate height in rounded pixels using the aspect ratio
					$(this).find('.jp-jplayer').width(containerWidth).height(videoHeight);
					// and then apply the width and height!
				});
			})
				.trigger('resize');
		}


		/**
		 * RD Instafeed JS
		 * @description Enables Instafeed JS
		 */
		if (plugins.instafeed.length > 0 && !livedemo) {
			for ( var i = 0; i < plugins.instafeed.length; i++) {
				var instafeedItem = $(plugins.instafeed[i]);
				instafeedItem.RDInstafeed({
					accessToken: '5526956400.ba4c844.c832b2a554764bc8a1c66c39e99687d7',
					clientId: 'c832b2a554764bc8a1c66c39e99687d7',
					userId: '5526956400',
					showLog: false
				});
			}
		}

		/**
		 * @desc Initialize the gallery with set of images
		 * @param {object} itemsToInit - jQuery object
		 * @param {string} addClass - additional gallery class
		 */
		function initLightGallery(itemsToInit, addClass) {
			if (!isNoviBuilder) {
				$(itemsToInit).lightGallery({
					thumbnail: $(itemsToInit).attr("data-lg-thumbnail") !== "false",
					selector: "[data-lightgallery='item']",
					autoplay: $(itemsToInit).attr("data-lg-autoplay") === "true",
					pause: parseInt($(itemsToInit).attr("data-lg-autoplay-delay")) || 5000,
					addClass: addClass,
					mode: $(itemsToInit).attr("data-lg-animation") || "lg-slide",
					loop: $(itemsToInit).attr("data-lg-loop") !== "false"
				});
			}
		}

		/**
		 * @desc Initialize the gallery with dynamic addition of images
		 * @param {object} itemsToInit - jQuery object
		 * @param {string} addClass - additional gallery class
		 */ 
		function initDynamicLightGallery(itemsToInit, addClass) {
			if (!isNoviBuilder) {
				$(itemsToInit).on("click", function () {
					$(itemsToInit).lightGallery({
						thumbnail: $(itemsToInit).attr("data-lg-thumbnail") !== "false",
						selector: "[data-lightgallery='item']",
						autoplay: $(itemsToInit).attr("data-lg-autoplay") === "true",
						pause: parseInt($(itemsToInit).attr("data-lg-autoplay-delay")) || 5000,
						addClass: addClass,
						mode: $(itemsToInit).attr("data-lg-animation") || "lg-slide",
						loop: $(itemsToInit).attr("data-lg-loop") !== "false",
						dynamic: true,
						dynamicEl: JSON.parse($(itemsToInit).attr("data-lg-dynamic-elements")) || []
					});
				});
			}
		}

		/**
		 * @desc Initialize the gallery with one image
		 * @param {object} itemToInit - jQuery object
		 * @param {string} addClass - additional gallery class
		 */
		function initLightGalleryItem(itemToInit, addClass) {
			if (!isNoviBuilder) {
				$(itemToInit).lightGallery({
					selector: "this",
					addClass: addClass,
					counter: false,
					youtubePlayerParams: {
						modestbranding: 1,
						showinfo: 0,
						rel: 0,
						controls: 0
					},
					vimeoPlayerParams: {
						byline: 0,
						portrait: 0
					}
				});
			}
		}

		// lightGallery
		if (plugins.lightGallery.length) {
			for (var i = 0; i < plugins.lightGallery.length; i++) {
				initLightGallery(plugins.lightGallery[i]);
			}
		}

		// lightGallery item
		if (plugins.lightGalleryItem.length) {
			// Filter carousel items
			var notCarouselItems = [];

			for (var z = 0; z < plugins.lightGalleryItem.length; z++) {
				if (!$(plugins.lightGalleryItem[z]).parents('.owl-carousel').length && !$(plugins.lightGalleryItem[z]).parents('.swiper-slider').length && !$(plugins.lightGalleryItem[z]).parents('.slick-slider').length) {
					notCarouselItems.push(plugins.lightGalleryItem[z]);
				}
			}

			plugins.lightGalleryItem = notCarouselItems;

			for (var i = 0; i < plugins.lightGalleryItem.length; i++) {
				initLightGalleryItem(plugins.lightGalleryItem[i]);
			}
		}

		// Dynamic lightGallery
		if (plugins.lightDynamicGalleryItem.length) {
			for (var i = 0; i < plugins.lightDynamicGalleryItem.length; i++) {
				initDynamicLightGallery(plugins.lightDynamicGalleryItem[i]);
			}
		}


		/**
		 * RD Twitter Feed
		 * @description Enables RD Twitter Feed plugin
		 */
		if (plugins.twitterfeed.length > 0) {
			for (var i = 0; i < plugins.twitterfeed.length; i++) {
				try {
					$(plugins.twitterfeed[i]).RDTwitter({});
				} catch (error) {
					console.warn(error);
				}
			}
		}

		/**
		 * RD Facebook
		 * @description Enables RD Facebook plugin
		 */
		if (plugins.facebookfeed.length > 0) {
			for (var i = 0; i < plugins.facebookfeed.length; i++) {
				var facebookfeedItem = plugins.facebookfeed[i];
				$(facebookfeedItem).RDFacebookFeed({});
			}
		}

		/**
		 * UI To Top
		 * @description Enables ToTop Button
		 */
		if (isDesktop && !isNoviBuilder) {
			$().UItoTop({
				easingType: 'easeOutQuart',
				containerClass: 'ui-to-top fa fa-angle-up'
			});
		}

		/**
		 * Circle Progress
		 * @description Enable Circle Progress plugin
		 */
		if (plugins.circleProgress.length) {
			var i;
			for (i = 0; i < plugins.circleProgress.length; i++) {
				var circleProgressItem = $(plugins.circleProgress[i]);
				$document
					.on("scroll", $.proxy(function () {
						var $this = $(this);

						if (!$this.hasClass('animated') && isScrolledIntoView($this)) {

							var arrayGradients = $this.attr('data-gradient').split(",");

							$this.circleProgress({
								value: $this.attr('data-value'),
								size: $this.attr('data-size') ? $this.attr('data-size') : 175,
								fill: {gradient: arrayGradients, gradientAngle: Math.PI / 4},
								startAngle: -Math.PI / 4 * 2,
								emptyFill: $this.attr('data-empty-fill') ? $this.attr('data-empty-fill') : "rgb(245,245,245)",
								thickness: $this.attr('data-thickness') ? parseInt($this.attr('data-thickness'), 10) : 10,

							}).on('circle-animation-progress', function (event, progress, stepValue) {
								$(this).find('span').text(String(stepValue.toFixed(2)).replace('0.', '').replace('1.', '1'));
							});
							$this.addClass('animated');
						}
					}, circleProgressItem))
					.trigger("scroll");
			}
		}

		/**
		 * jQuery Countdown
		 * @description  Enable countdown plugin
		 */
		if (plugins.countDown.length) {
			var i;
			for (i = 0; i < plugins.countDown.length; i++) {
				var $countDownItem = $(plugins.countDown[i]),
					d = new Date(),
					type = $countDownItem.attr('data-type'),
					time = $countDownItem.attr('data-time'),
					format = $countDownItem.attr('data-format'),
					settings = [];

				d.setTime(Date.parse(time)).toLocaleString();
				settings[type] = d;
				settings['format'] = format;
				$countDownItem.countdown(settings);
			}
		}

		/**
		 * SVG Countdown
		 */
		if (plugins.svgCountDown.length) {
			svgCountDown({
				tickInterval: 100,
				counterSelector: '.countdown-counter'
			});
		}

		/**
		 * Textillate.js
		 * */

		if (!isNoviBuilder && plugins.animtext.length) {
			for (var i = 0; i < plugins.animtext.length; i++) {
				$(plugins.animtext[i]).textillate({
					selector: '.textillate-text',
				});
			}
		}

		/**
		 * typedjs
		 */
		if (!isNoviBuilder && plugins.typedjs.length) {
			var typed = new Typed(".typed-text", {
				stringsElement: ".typed-strings",
				typeSpeed: 80,
				loop: !0,
				backDelay: 1500,
				backSpeed: 80
			});
		}

		/**
		 * Slide down toggle
		 */
		if (plugins.showMoreToggle.length) {
			for (var i = 0; i < plugins.showMoreToggle.length; i++) {
				$(plugins.showMoreToggle[i]).on('click', function () {
					$(this).parents('.slide-down-wrapper').children('.hidden-content').slideToggle(1000, "linear");
					$(this).toggleClass('active');
				});
			}
		}

		/**
		 * button flip for builder
		 */
		if (plugins.animBox.length && isNoviBuilder) {
			var newBtn = "<button type='button' class='flip btn btn-primary'>Click to Flip</button>";

			for (var i = 0; i < plugins.animBox.length; i++) {
				var animWrap = $(plugins.animBox[i]);
				animWrap.append(newBtn);
				animWrap.find('.flip').on('click', function (animWrap) {
					return function () {
						animWrap.toggleClass('active');
					};
				}(animWrap));
			}
		}

		/**
		 * Google maps
		 * */
		if (plugins.maps.length) {
		  	lazyInit( plugins.maps, initMaps );
		}
	
		function initMaps() {
			var key;

			for ( var i = 0; i < plugins.maps.length; i++ ) {
				if ( plugins.maps[i].hasAttribute( "data-key" ) ) {
					key = plugins.maps[i].getAttribute( "data-key" );
					break;
				}
			}

			$.getScript('//maps.google.com/maps/api/js?'+ ( key ? 'key='+ key + '&' : '' ) +'sensor=false&libraries=geometry,places&v=quarterly', function () {
				var head = document.getElementsByTagName('head')[0],
					insertBefore = head.insertBefore;

				head.insertBefore = function (newElement, referenceElement) {
					if (newElement.href && newElement.href.indexOf('//fonts.googleapis.com/css?family=Roboto') !== -1 || newElement.innerHTML.indexOf('gm-style') !== -1) {
						return;
					}
					insertBefore.call(head, newElement, referenceElement);
				};
				var geocoder = new google.maps.Geocoder;
				for (var i = 0; i < plugins.maps.length; i++) {
					var zoom = parseInt(plugins.maps[i].getAttribute("data-zoom"), 10) || 11;
					var styles = plugins.maps[i].hasAttribute('data-styles') ? JSON.parse(plugins.maps[i].getAttribute("data-styles")) : [];
					var center = plugins.maps[i].getAttribute("data-center") || "New York";

					// Initialize map
					var map = new google.maps.Map(plugins.maps[i].querySelectorAll(".google-map")[0], {
						zoom: zoom,
						styles: styles,
						scrollwheel: false,
						center: {lat: 0, lng: 0}
					});

					// Add map object to map node
					plugins.maps[i].map = map;
					plugins.maps[i].geocoder = geocoder;
					plugins.maps[i].keySupported = true;
					plugins.maps[i].google = google;

					// Get Center coordinates from attribute
					getLatLngObject(center, null, plugins.maps[i], function (location, markerElement, mapElement) {
						mapElement.map.setCenter(location);
					});

					// Add markers from google-map-markers array
					var markerItems = plugins.maps[i].querySelectorAll(".google-map-markers li");

					if (markerItems.length){
						var markers = [];
						for (var j = 0; j < markerItems.length; j++){
							var markerElement = markerItems[j];
							getLatLngObject(markerElement.getAttribute("data-location"), markerElement, plugins.maps[i], function(location, markerElement, mapElement){
								var icon = markerElement.getAttribute("data-icon") || mapElement.getAttribute("data-icon");
								var activeIcon = markerElement.getAttribute("data-icon-active") || mapElement.getAttribute("data-icon-active");
								var info = markerElement.getAttribute("data-description") || "";
								var infoWindow = new google.maps.InfoWindow({
									content: info
								});
								markerElement.infoWindow = infoWindow;
								var markerData = {
									position: location,
									map: mapElement.map
								}
								if (icon){
									markerData.icon = icon;
								}
								var marker = new google.maps.Marker(markerData);
								markerElement.gmarker = marker;
								markers.push({markerElement: markerElement, infoWindow: infoWindow});
								marker.isActive = false;
								// Handle infoWindow close click
								google.maps.event.addListener(infoWindow,'closeclick',(function(markerElement, mapElement){
									var markerIcon = null;
									markerElement.gmarker.isActive = false;
									markerIcon = markerElement.getAttribute("data-icon") || mapElement.getAttribute("data-icon");
									markerElement.gmarker.setIcon(markerIcon);
								}).bind(this, markerElement, mapElement));


								// Set marker active on Click and open infoWindow
								google.maps.event.addListener(marker, 'click', (function(markerElement, mapElement) {
									if (markerElement.infoWindow.getContent().length === 0) return;
									var gMarker, currentMarker = markerElement.gmarker, currentInfoWindow;
									for (var k =0; k < markers.length; k++){
										var markerIcon;
										if (markers[k].markerElement === markerElement){
											currentInfoWindow = markers[k].infoWindow;
										}
										gMarker = markers[k].markerElement.gmarker;
										if (gMarker.isActive && markers[k].markerElement !== markerElement){
											gMarker.isActive = false;
											markerIcon = markers[k].markerElement.getAttribute("data-icon") || mapElement.getAttribute("data-icon")
											gMarker.setIcon(markerIcon);
											markers[k].infoWindow.close();
										}
									}

									currentMarker.isActive = !currentMarker.isActive;
									if (currentMarker.isActive) {
										if (markerIcon = markerElement.getAttribute("data-icon-active") || mapElement.getAttribute("data-icon-active")){
											currentMarker.setIcon(markerIcon);
										}

										currentInfoWindow.open(map, marker);
									}else{
										if (markerIcon = markerElement.getAttribute("data-icon") || mapElement.getAttribute("data-icon")){
											currentMarker.setIcon(markerIcon);
										}
										currentInfoWindow.close();
									}
								}).bind(this, markerElement, mapElement))
							})
						}
					}
				}
			});
		}


		/**
		 * add active in Services tabs
		 * */
		if (plugins.modal.length) {
			$('.onepage-service').find('[data-toggle="modal"]').on('click', function () {
				plugins.modal.find('.nav-link').removeClass('active');
				plugins.modal.find('.tab-pane').removeClass('active show');

				plugins.modal.find('[href="#tabs-1-1"]').addClass('active');
				plugins.modal.find('#tabs-1-1').addClass('active show');
			});

			$('.multipage-service').find('[data-toggle="modal"]').on('click', function () {
				plugins.modal.find('.nav-link').removeClass('active');
				plugins.modal.find('.tab-pane').removeClass('active show');

				plugins.modal.find('[href="#tabs-1-2"]').addClass('active');
				plugins.modal.find('#tabs-1-2').addClass('active show');
			});

			$('.extended-service').find('[data-toggle="modal"]').on('click', function () {
				plugins.modal.find('.nav-link').removeClass('active');
				plugins.modal.find('.tab-pane').removeClass('active show');

				plugins.modal.find('[href="#tabs-1-3"]').addClass('active');
				plugins.modal.find('#tabs-1-3').addClass('active show');
			});
		}


		/**
		 * canvas animation
		 */
		if (plugins.particles.length) {
			particlesJS("particles-js", {
				"particles": {
					"number": {
						"value": 230,
						"density": {
							"enable": true,
							"value_area": 5000
						}
					},
					"color": {
						"value": "#ffffff"
					},
					"shape": {
						"type": "circle",
						"stroke": {
							"width": 0,
							"color": "#000000"
						},
						"polygon": {
							"nb_sides": 5
						},
						"image": {
							"src": "img/github.svg",
							"width": 100,
							"height": 100
						}
					},
					"opacity": {
						"value": 0.5,
						"random": false,
						"anim": {
							"enable": false,
							"speed": 1,
							"opacity_min": 0.1,
							"sync": false
						}
					},
					"size": {
						"value": 5,
						"random": true,
						"anim": {
							"enable": false,
							"speed": 10,
							"size_min": 0.1,
							"sync": false
						}
					},
					"line_linked": {
						"enable": true,
						"distance": 150,
						"color": "#ffffff",
						"opacity": 0.4,
						"width": 1
					},
					"move": {
						"enable": true,
						"speed": 3,
						"direction": "none",
						"random": false,
						"straight": false,
						"out_mode": "out",
						"bounce": false,
						"attract": {
							"enable": false,
							"rotateX": 600,
							"rotateY": 1200
						}
					}
				},
				"interactivity": {
					"detect_on": "canvas",
					"events": {
						"onhover": {
							"enable": true,
							"mode": "grab"
						},
						"onclick": {
							"enable": true,
							"mode": "push"
						},
						"resize": true
					},
					"modes": {
						"grab": {
							"distance": 140,
							"line_linked": {
								"opacity": 1
							}
						},
						"bubble": {
							"distance": 400,
							"size": 40,
							"duration": 2,
							"opacity": 8,
							"speed": 3
						},
						"repulse": {
							"distance": 200,
							"duration": 0.4
						},
						"push": {
							"particles_nb": 4
						},
						"remove": {
							"particles_nb": 2
						}
					}
				},
				"retina_detect": true
			});
		}

		/**
		 * Parallax on first section
		 */

		if (plugins.parallaxTextBody.length) {
			var inner = document.querySelector(".parallax-section-inner");
			var zInner, o,
				heightParLayer,
				innerHeight,
				offsetTop;

			parallaxScroll();
			window.addEventListener("resize", function () {
				parallaxScroll();
			}); 
			window.addEventListener('scroll', function () {
				parallaxScroll();
			});
		}

	});

}());
