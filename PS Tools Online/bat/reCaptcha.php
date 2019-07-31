<?php
// Initiate the autoloader.
require_once 'ReCaptcha/autoload.php';

// Register You API keys at https://www.google.com/recaptcha/admin
// And write it here
$siteKey = '6LfZlSETAAAAAC5VW4R4tQP8Am_to4bM3dddxkEt';
$secret = '6LfZlSETAAAAAOi4lh7GHcSOO0pbXnAMJRhnsr7O';

// reCAPTCHA supported 40+ languages listed here: https://developers.google.com/recaptcha/docs/language
$lang = 'en';

// If No key
if ($siteKey === '' || $secret === ''):
  die('CPT001');
elseif (isset($_POST['g-recaptcha-response'])):

  // If the form submission includes the "g-captcha-response" field
  // Create an instance of the service using your secret
  $recaptcha = new \ReCaptcha\ReCaptcha($secret);

  // Make the call to verify the response and also pass the user's IP address
  $resp = $recaptcha->verify($_POST['g-recaptcha-response'], $_SERVER['REMOTE_ADDR']);

  if ($resp->isSuccess()):
    // If the response is a success, that's it!
    die('CPT000');
  else:
    // Something wrong
    die('CPT002');
  endif;

endif;
?>
