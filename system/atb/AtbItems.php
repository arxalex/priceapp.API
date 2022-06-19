<?php

namespace atb;

use DOMDocument;
use DOMXPath;
use framework\database\NumericHelper;
use framework\entities\items\AtbItem;
use framework\entities\items\AtbItemsService;
use framework\entities\categories\AtbCategoriesService;
use framework\entities\categories\AtbCategory;
use framework\entities\filials\AtbFilialsService;
use framework\entities\prices\Price;
use framework\entities\prices\PricesService;

class AtbItems
{
	private AtbCategoriesService $_atbCategoriesService;
	private AtbItemsService $_atbItemsService;
	private PricesService $_pricesService;
	private AtbFilialsService $_atbFilialsService;

	public function __construct()
	{
		$this->_atbCategoriesService = new AtbCategoriesService();
		$this->_atbItemsService = new AtbItemsService();
		$this->_pricesService = new PricesService();
		$this->_atbFilialsService = new AtbFilialsService();
	}

	public function loadItemsByCategoryAndFilial(int $categoryId, int $filialId = 1154, bool $onlyNew = true): int
	{
		$itemIds = $this->loadItemIds($categoryId, $filialId);
		$items = [];
		foreach ($itemIds as $itemId) {
			if (!$onlyNew || ($onlyNew && $this->_atbItemsService->count(['internalid' => [$itemId]]) <= 0)) {
				$item = $this->loadProductInfo($itemId, $categoryId, $filialId);
				if($item->internalid == null){
					continue;
				}

				$this->_atbItemsService->insertItemToDB($item);

				$items[] = $item;
			}
		}

		return count($items);
	}

	public function loadItemsPricesByCategoryAndFilial(int $categoryId, int $filialId = 1154): int
	{
		$itemIdsAndPrices = [];
		$i = 0;
		$next_page = true;
		libxml_use_internal_errors(true);

		while ($next_page) {
			$url = "https://zakaz.atbmarket.com/shop/catalog/wloadmore?cat=$categoryId&store=$filialId&page=$i";

			$options = [
				'http' => [
					'method' => 'GET',
					'header' => "Cookie: store_id=$filialId\r\n",
				],
				'ssl' => [
					'verify_peer' => false,
					'verify_peer_name' => false
				]
			];
			$context  = stream_context_create($options);
			$result = json_decode(file_get_contents($url, false, $context));

			$html = new DOMDocument();
			if($result->markup == ''){
				break;
			}
			$html->loadHTML($result->markup);
			$xpath = new DOMXPath($html);
			$xpathQuery = "//article";
			$arrayOfProducts = $xpath->query($xpathQuery);


			foreach ($arrayOfProducts as $product) {
				$productDOM = new DOMDocument();
				$cloned = $product->cloneNode(true);
				$productDOM->appendChild($productDOM->importNode($cloned, true));
				$xpath = new DOMXPath($productDOM);

				$priceQuery = $xpath->query("//div[@class='catalog-item__bottom']/div[contains(@class, 'catalog-item__product-price')]/data");
				$priceOfItem = $priceQuery[0]->getAttribute('value');

				$itemIdQuery = $xpath->query("//div[@class='catalog-item__title']/a");
				$itemId = (explode('/', $itemIdQuery[0]->getAttribute('href')))[3];

				$filialIdFromDB = ($this->_atbFilialsService->getItemsFromDB(['inshopid' => [$filialId]]))[0]->id;
				$itemIdsFromDB = $this->_atbItemsService->getItemsFromDB(['internalid' => [$itemId]]);
				if (count($itemIdsFromDB) > 0) {
					$itemIdFromDB = $itemIdsFromDB[0]->id;
				} else {
					$itemInfo = $this->loadProductInfo($itemId, $categoryId, $filialId);
					if($itemInfo->internalid == null){
						continue;
					}
					$this->_atbItemsService->insertItemToDB($itemInfo);
					$itemIdsFromDB = $this->_atbItemsService->getItemsFromDB(['internalid' => [$itemId]]);
					$itemIdFromDB = $itemIdsFromDB[0]->id;
				}

				$price = new Price();
				$price->itemid = $itemIdFromDB;
				$price->shopid = 3;
				$price->price = NumericHelper::toFloat($priceOfItem);
				$price->filialid = $filialIdFromDB;
				$price->quantity = 1;

				if ($this->_pricesService->count(['itemid' => [$itemIdFromDB], 'shopid' => [3], 'filialid' => [$filialIdFromDB]]) > 0) {
					$priceId = ($this->_pricesService->getItemsFromDB(['itemid' => [$itemIdFromDB], 'shopid' => [3], 'filialid' => [$filialIdFromDB]]))[0]->id;
					$price->id = $priceId;
					$this->_pricesService->updateItemInDB($price);
				} else {
					$this->_pricesService->insertItemToDB($price);
				}

				$itemIdsAndPrices[] = $price;
			}
			$i++;
			$next_page = $result->next_page;
		}

		return count($itemIdsAndPrices);
	}

	private function loadItemIds(int $categoryId, int $filialId = 1154): array
	{
		$itemIds = [];
		$i = 0;
		$next_page = true;
		libxml_use_internal_errors(true);

		while ($next_page) {
			$url = "https://zakaz.atbmarket.com/shop/catalog/wloadmore?cat=$categoryId&store=$filialId&page=$i";

			$options = [
				'http' => [
					'method' => 'GET',
					'header' => "Cookie: store_id=$filialId\r\n",
				],
				'ssl' => [
					'verify_peer' => false,
					'verify_peer_name' => false
				]
			];
			$context  = stream_context_create($options);
			$result = json_decode(file_get_contents($url, false, $context));

			$html = new DOMDocument();
			$html->loadHTML($result->markup);
			$xpath = new DOMXPath($html);
			$xpathQuery = "//article";
			$arrayOfProducts = $xpath->query($xpathQuery);


			foreach ($arrayOfProducts as $product) {
				$productDOM = new DOMDocument();
				$cloned = $product->cloneNode(true);
				$productDOM->appendChild($productDOM->importNode($cloned, true));
				$xpath = new DOMXPath($productDOM);

				$itemIdQuery = $xpath->query("//div[@class='catalog-item__title']/a");
				$itemIds[] = (explode('/', $itemIdQuery[0]->getAttribute('href')))[3];
			}
			$i++;
			$next_page = $result->next_page;
		}

		return $itemIds;
	}

	private function loadProductInfo(int $itemId, int $parentCategory, int $filialId = 1154): AtbItem
	{
		$item = new AtbItem();
		$html = new DOMDocument();
		$context = stream_context_create([
			'http' => [
				'method' => 'GET',
				'header' => "Cookie: store_id=$filialId\r\n",
			],
			"ssl" => [
				'verify_peer' => false,
				'verify_peer_name' => false
			]
		]);

		libxml_set_streams_context($context);
		libxml_use_internal_errors(true);
		$html->loadHTMLFile("https://zakaz.atbmarket.com/product/$filialId/$itemId");
		if ($http_response_header[0] == "HTTP/1.1 404 Not Found") {
			return new AtbItem();
		}
		$xpath = new DOMXPath($html);

		$item->internalid = $itemId;

		$xpathNameQuery = "//h1[@class='page-title']";
		$item->label = $xpath->query($xpathNameQuery)[0]->nodeValue;

		$xpathImageQuery = "//div[@class='cardproduct-tabs__item']/picture/source";
		$imageAttribute = $xpath->query($xpathImageQuery);
		if (count($imageAttribute) > 0 && $imageAttribute[0] != null) {
			$item->image = $imageAttribute[0]->getAttribute('srcset');
		}

		$xpathCategoryQuery = "//li[@class='breadcrumbs__item']/a";
		$categories = $xpath->query($xpathCategoryQuery);
		$category = $categories[count($categories) - 1];
		$internalCategoryId = (explode('/', $category->getAttribute('href')))[3];
		$categoryArrSearch = $this->_atbCategoriesService->getItemsFromDB(['internalid' => [$internalCategoryId]]);
		if(count($categoryArrSearch) > 0){
			$item->category = $categoryArrSearch[0]->id;
		} else {
			$newCategory = new AtbCategory();
			$newCategory->internalid = $internalCategoryId;
			$newCategory->label = $category->nodeValue;
			$newCategory->parent = $parentCategory;
			$this->_atbCategoriesService->insertItemToDB($newCategory);
			$categoryArrSearch = $this->_atbCategoriesService->getItemsFromDB(['internalid' => [$internalCategoryId]]);
			$item->category = $categoryArrSearch[0]->id;
		}

		$xpathCharacteristicsQuery = "//div[@class='product-characteristics__item']";

		$arrayOfCharacteristics = $xpath->query($xpathCharacteristicsQuery);

		foreach ($arrayOfCharacteristics as $characteristic) {
			$characteristicDOM = new DOMDocument();
			$cloned = $characteristic->cloneNode(true);
			$characteristicDOM->appendChild($characteristicDOM->importNode($cloned, true));
			$xpath = new DOMXPath($characteristicDOM);

			$characteristicName = $xpath->query("//div[@class='product-characteristics__name']")[0]->nodeValue;
			$characteristicValue = $xpath->query("//div[@class='product-characteristics__value']")[0]->nodeValue;
			if ($characteristicName == "Країна") {
				$item->country = $characteristicValue;
			} elseif ($characteristicName == "Торгова марка") {
				$item->brand = $characteristicValue;
			}
		}

		return $item;
	}
}
