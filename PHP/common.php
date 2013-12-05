<?php

/*
 * Base URL to the REST service
 */
define('BASE_URL', 'http://api.edipost.no');


/*
 * API key for authentication
 */
define('API_KEY', '9953713dc7d97f7f7883b902e8205adf7ca1380e');


/*
 * Do GET requests
 */
function getRequest( $url, $apiKey, $headers ) {
	$ch = curl_init($url);

	curl_setopt($ch, CURLOPT_FOLLOWLOCATION, TRUE);
	curl_setopt($ch, CURLOPT_USERPWD, "api:$apiKey");
	curl_setopt($ch, CURLOPT_HTTPHEADER, $headers);
	curl_setopt($ch, CURLINFO_HEADER_OUT, true);
	curl_setopt($ch, CURLOPT_RETURNTRANSFER, TRUE);
	curl_setopt($ch, CURLOPT_ENCODING, '');
	curl_setopt($ch, CURLOPT_TIMEOUT, 15);
	//curl_setopt($ch, CURLOPT_VERBOSE, true);

	$data = curl_exec($ch);
	$info = curl_getinfo($ch);

	if( $info['http_code'] != 200 ) {
		throw new Exception('Error when getting ' . $url . '. Message: "' . strip_tags($data) . '"" (' . $info['http_code'] . ')');
	}

	return $data;
}

/*
 * Do HTTP POST requests
 */
function postRequest( $url, $apiKey, $headers, $body ) {
	$ch = curl_init($url);

	curl_setopt($ch, CURLOPT_FOLLOWLOCATION, TRUE);
	curl_setopt($ch, CURLOPT_USERPWD, "api:$apiKey");
	curl_setopt($ch, CURLOPT_HTTPHEADER, $headers);
	curl_setopt($ch, CURLOPT_POSTFIELDS, $body);
	curl_setopt($ch, CURLOPT_POST, TRUE);
	curl_setopt($ch, CURLINFO_HEADER_OUT, true);
	curl_setopt($ch, CURLOPT_RETURNTRANSFER, TRUE);
	curl_setopt($ch, CURLOPT_ENCODING, '');
	curl_setopt($ch, CURLOPT_TIMEOUT, 15);
	curl_setopt($ch, CURLOPT_VERBOSE, true);

	$data = curl_exec($ch);
	$info = curl_getinfo($ch);

	// Make sure the request return HTTP 201 (Created) when creating new consignees
	if( $info['http_code'] != 201 ) {
		throw new Exception('Error when posting to ' . $url . '. Message: "' . strip_tags($data) . '"" (' . $info['http_code'] . ')');
	}

	return $data;
}

?>