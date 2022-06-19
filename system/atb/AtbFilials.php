<?php

namespace atb;

use DOMDocument;
use DOMXPath;
use framework\database\ListHelper;
use framework\database\NumericHelper;
use framework\database\StringHelper;
use framework\entities\filials\AtbFilialsService;
use framework\entities\filials\AtbFilial;

class AtbFilials
{
	private AtbFilialsService $_atbFilialsService;
	public function __construct()
	{
		$this->_atbFilialsService = new AtbFilialsService();
	}

	public function loadFilials(bool $onlyNew = true): int
	{
		$html = new DOMDocument();
		$context = stream_context_create(array(
			"ssl" => array(
				'verify_peer' => false,
				'verify_peer_name' => false
			)
		));

		libxml_set_streams_context($context);
		libxml_use_internal_errors(true);
		$html->loadHTMLFile("https://zakaz.atbmarket.com/");
		$xpath = new DOMXPath($html);

		$xpathCategoryQuery = "//*[@id='region']/option";
		$regionDOMElements = $xpath->query($xpathCategoryQuery);

		$filials = [];

		foreach ($regionDOMElements as $regionDOMElement) {
			$regionName = $regionDOMElement->nodeValue;
			$regionId = $regionDOMElement->getAttribute('value');

			if ($regionId == '') {
				continue;
			}

			$url = "https://zakaz.atbmarket.com/site/getregion";
			$data = [
				'id' => $regionId
			];
			$data = http_build_query($data);
			$options = [
				'http' => [
					'header' => "X-Requested-With: XMLHttpRequest\r\n" .
						"Content-Type: application/x-www-form-urlencoded\r\n" .
						"Content-Length: " . strlen($data) . "\r\n",
					'method' => 'POST',
					'content' => $data
				],
				'ssl' => [
					'verify_peer' => false,
					'verify_peer_name' => false
				]
			];
			$context  = stream_context_create($options);
			$resultRegion = file_get_contents($url, false, $context);

			$htmlRegion = new DOMDocument();
			$htmlRegion->loadHTML($resultRegion);
			$xpathRegion = new DOMXPath($htmlRegion);

			$citiesDOMs = $xpathRegion->query("//option");
			foreach ($citiesDOMs as $citiesDOM) {
				$cityName = utf8_decode($citiesDOM->nodeValue);
				$cityId = $citiesDOM->getAttribute('value');

				if ($cityId == '') {
					continue;
				}

				$url = "https://zakaz.atbmarket.com/site/getstore";
				$data = [
					'store' => $cityId
				];
				$data = http_build_query($data);
				$options = [
					'http' => [
						'header' => "X-Requested-With: XMLHttpRequest\r\n" .
							"Content-Type: application/x-www-form-urlencoded\r\n" .
							"Content-Length: " . strlen($data) . "\r\n",
						'method' => 'POST',
						'content' => $data
					],
					'ssl' => [
						'verify_peer' => false,
						'verify_peer_name' => false
					]
				];
				$context  = stream_context_create($options);
				$resultCity = json_decode(file_get_contents($url, false, $context));
				if ($http_response_header[0] == "HTTP/1.1 302 Found") {
					$filialId = explode('/', $http_response_header[4])[3];

					if (!$onlyNew || ($onlyNew && $this->_atbFilialsService->count(['inshopid' => [$filialId]]) <= 0)) {
						$url = "https://zakaz.atbmarket.com/shop/catalog/wdelivery?store_id=$filialId";
						$options = [
							'http' => [
								'method' => 'GET'
							],
							'ssl' => [
								'verify_peer' => false,
								'verify_peer_name' => false
							]
						];
						$context  = stream_context_create($options);
						$resultFilial = json_decode(file_get_contents($url, false, $context));

						$streetAndHouse = explode(" ", $resultFilial->out->address, 2)[1];
						$streetAndHouseArr = explode(", ", $streetAndHouse, 2);

						$filial = new AtbFilial();
						$filial->inshopid = $filialId;
						$filial->city = $cityName;
						$filial->region = $regionName . " обл.";
						$filial->street = $streetAndHouseArr[0];
						$filial->house = $streetAndHouseArr[1];
						$filial->label = explode(" ", $resultFilial->out->address, 2)[0];

						$this->_atbFilialsService->insertItemToDB($filial);
						
						$filials[] = $filial;
					}
				} else {
					$htmlCity = new DOMDocument();
					$htmlCity->loadHTML($resultCity->optselect);
					$xpathCity = new DOMXPath($htmlCity);

					$filialDOMs = $xpathCity->query("//option");
					foreach ($filialDOMs as $filialDOM) {
						$filialAddress = utf8_decode($filialDOM->nodeValue);
						$filialId = $filialDOM->getAttribute('value');

						if ($filialId == '') {
							continue;
						}

						if (!$onlyNew || ($onlyNew && $this->_atbFilialsService->count(['inshopid' => [$filialId]]) <= 0)) {
							$streetAndHouseArr = explode(", ", $filialAddress, 2);

							$url = "https://zakaz.atbmarket.com/shop/catalog/wdelivery?store_id=$filialId";
							$options = [
								'http' => [
									'method' => 'GET'
								],
								'ssl' => [
									'verify_peer' => false,
									'verify_peer_name' => false
								]
							];
							$context  = stream_context_create($options);
							$resultFilial = json_decode(file_get_contents($url, false, $context));

							$cord = ListHelper::getOneByFields($resultCity->coordinates, ['id' => $filialId]);

							$filial = new AtbFilial();
							$filial->inshopid = $filialId;
							$filial->city = $cityName;
							$filial->region = $regionName . " обл.";
							$filial->street = $streetAndHouseArr[0];
							$filial->house = $streetAndHouseArr[1];
							$filial->label = explode(" ", $resultFilial->out->address, 2)[0];
							$filial->xcord = NumericHelper::toFloatOrNull($cord->lng);
							$filial->ycord = NumericHelper::toFloatOrNull($cord->lat);

							$this->_atbFilialsService->insertItemToDB($filial);

							$filials[] = $filial;
						}
					}
				}
			}
		}

		return count($filials);
	}
}
