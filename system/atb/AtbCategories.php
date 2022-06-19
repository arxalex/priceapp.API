<?php

namespace atb;

use DOMDocument;
use DOMXPath;
use framework\database\StringHelper;
use framework\entities\categories\AtbCategoriesService;
use framework\entities\categories\AtbCategory;

class AtbCategories
{
	private AtbCategoriesService $_atbCategoriesService;
	public function __construct()
	{
		$this->_atbCategoriesService = new AtbCategoriesService();
	}

	public function loadCategories(bool $onlyNew = true): int
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

		$xpathCategoryQuery = "//li[@class='category-menu__item']";
		$categoryDOMElements = $xpath->query($xpathCategoryQuery);

		$categories = [];
		foreach ($categoryDOMElements as $categoryDOMElement) {

			$xpathCategoryQuery = "//a[@class='category-menu__link']";
			$categoryDOM = new DOMDocument();
			$cloned = $categoryDOMElement->cloneNode(true);
			$categoryDOM->appendChild($categoryDOM->importNode($cloned, true));
			$xpathCategoryDOM = new DOMXPath($categoryDOM);
			$categoryLink = $xpathCategoryDOM->query($xpathCategoryQuery)[0];
			$categoryId = (explode('/', $categoryLink->getAttribute('href')))[3];
			if($categoryId == 'economy' || StringHelper::stringContains($categoryId, 'novetly') || $categoryId == 388){
				continue;
			}

			if(!$onlyNew || ($onlyNew && $this->_atbCategoriesService->count(['internalid' => [$categoryId]]) <= 0)){
				$category = new AtbCategory();
				$category->internalid = $categoryId;
				$category->label = $categoryLink->nodeValue;
				$this->_atbCategoriesService->insertItemToDB($category);

				$categories[] = $category;
			}

			$xpathSubCategoryQuery = "//a[@class='submenu__link']";
			$subCategoriesLinks = $xpathCategoryDOM->query($xpathSubCategoryQuery);

			foreach($subCategoriesLinks as $subCategoryLink){
				$subCategoryId = (explode('/', $subCategoryLink->getAttribute('href')))[3];
	
				if(!$onlyNew || ($onlyNew && $this->_atbCategoriesService->count(['internalid' => [$subCategoryId]]) <= 0)){
					$subCategory = new AtbCategory();
					$subCategory->internalid = $subCategoryId;
					$subCategory->label = $subCategoryLink->nodeValue;
					$subCategory->parent = ($this->_atbCategoriesService->getItemsFromDB(['internalid' => [$categoryId]]))[0]->id;
					$this->_atbCategoriesService->insertItemToDB($subCategory);

					$categories[] = $subCategory;
				}
			}
		}

		return count($categories);
	}
}
