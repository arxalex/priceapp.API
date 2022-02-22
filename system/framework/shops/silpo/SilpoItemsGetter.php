<?php

namespace framework\shops\silpo;

use framework\database\ListHelper;
use framework\database\NumericHelper;
use framework\entities\brands\BrandsService;
use framework\entities\categories\CategoriesService;
use framework\entities\items\Item;
use framework\entities\categories_link\CategoriesLinkService;
use framework\entities\countries\CountriesService;
use framework\entities\items_link\ItemsLinkService;
use framework\entities\packages\PackagesService;
use framework\shops\silpo\SilpoItemModel;
use stdClass;

class SilpoItemsGetter
{
    private CategoriesService $_categoriesService;
    private CategoriesLinkService $_categoriesLinkService;
    private PackagesService $_packageService;
    private BrandsService $_brandService;
    private CountriesService $_countriesService;
    private ItemsLinkService $_itemsLinkService;

    public function __construct()
    {
        $this->_categoriesLinkService = new CategoriesLinkService();
        $this->_categoriesService = new CategoriesService();
        $this->_packageService = new PackagesService();
        $this->_brandService = new BrandsService();
        $this->_countriesService = new CountriesService();
        $this->_itemsLinkService = new ItemsLinkService();
    }
    public function get(int $categotyId, int $from = 0, int $to = 25, int $fillialId = 2043): array
    {
        $url = 'https://api.catalog.ecom.silpo.ua/api/2.0/exec/EcomCatalogGlobal';
        $data = json_encode([
            'data' => [
                'From' => $from,
                'To' => $to,
                'categoryId' => $categotyId,
                'filialId' => $fillialId,
            ],
            'method' => 'GetSimpleCatalogItems'
        ]);

        $options = [
            'http' => [
                'header'  => "Content-Type: application/json;charset=UTF-8\r\n",
                'method'  => 'POST',
                'content' => $data
            ]
        ];
        $context  = stream_context_create($options);
        $result = json_decode(file_get_contents($url, false, $context));
        $inTableItems = ListHelper::getColumn($this->_itemsLinkService->getItemsFromDB([
            'inshopid' => ListHelper::getColumn($result->items, 'id')
        ]), 'inshopid');
        $handledResult = $result->items;
        if (count($inTableItems) > 0) {
            foreach ($handledResult as $key => $value) {
                if(in_array($value->id, $inTableItems)){
                    unset($handledResult[$key]);
                }
            }
        }
        $items = [];
        foreach ($handledResult as $value) {
            $package = $this->getShopItemParam($value, 'packageType');
            $units = NumericHelper::toFloatOrNull($this->getShopItemParam($value, 'numberOfUnits'), true);
            if ($this->getShopItemParam($value, 'isWeighted') == "") {
                $package = "на вагу";
            }
            if (substr($value->unit, -4) == "кг") {
                $units = NumericHelper::toFloatOrNull(substr($value->unit, 0, -4), true);
            } elseif (substr($value->unit, -2) == "г") {
                $units = NumericHelper::toFloatOrNull(substr($value->unit, 0, -2), true) / 1000;
            } elseif (substr($value->unit, -9) == "шт/уп") {
                $package = "упаковка";
                $units = NumericHelper::toFloatOrNull(substr($value->unit, 0, -9), true);
            }
            $items[] = new SilpoItemModel(
                $value->id,
                $value->name,
                $value->mainImage,
                (($value->categories)[count($value->categories) - 1])->id,
                $this->getShopItemParam($value, 'trademark'),
                $package,
                NumericHelper::toFloatOrNull($this->getShopItemParam($value, 'alcoholContent'), true),
                $units,
                NumericHelper::toFloatOrNull($this->calorieConverter($this->getShopItemParam($value, 'calorie')), true),
                NumericHelper::toFloatOrNull($this->getShopItemParam($value, 'carbohydrates'), true),
                NumericHelper::toFloatOrNull($this->getShopItemParam($value, 'fats'), true),
                NumericHelper::toFloatOrNull($this->getShopItemParam($value, 'proteins'), true),
                $this->getShopItemParam($value, 'country'),
                "https://shop.silpo.ua/product/" . $value->slug
            );
        }
        return $items;
    }
    private function getShopItemParam($item, string $param, bool $nullOrFalse = true)
    {
        $isok = false;
        if ($item->parameters) {
            foreach ($item->parameters as $value) {
                if ($value->key == $param) {
                    $isok = true;
                    $tmp = $value->value;
                }
            }
        }
        if ($isok) {
            return $tmp;
        } else {
            if ($nullOrFalse) {
                return null;
            } else {
                return false;
            }
        }
    }
    public function convertFromSilpoToCommonModel(SilpoItemModel $silpoItem)
    {
        $baseCategoryLinks = $this->_categoriesLinkService->getItemsFromDB([
            'categoryshopid' => [$silpoItem->shopcategoryid]
        ]);
        $baseCategoryLink = count($baseCategoryLinks) > 0 ? $baseCategoryLinks[0] : null;
        $baseCategoryId = $baseCategoryLink !== null ? $baseCategoryLink->categoryid : null;

        $baseCategory = $baseCategoryId !== null ? $this->_categoriesService->getItemFromDB($baseCategoryId) : null;
        $category = $this->_categoriesService->getCategoryByName($silpoItem->label, $baseCategory);
        $brand = $this->_brandService->getBrand($silpoItem->brand);
        $package = $this->_packageService->getPackage($silpoItem->package);
        $country = $this->_countriesService->getCountry($silpoItem->country);
        $commonItem = new Item(
            null,
            $silpoItem->label,
            $silpoItem->image,
            $category->id,
            $brand->id,
            $package->id,
            $silpoItem->units,
            null,
            null,
            null,
            $silpoItem->calorie,
            $silpoItem->carbohydrates,
            $silpoItem->fat,
            $silpoItem->proteins,
            [
                'country' => $country->id,
                'alcohol' => $silpoItem->alcohol
            ]
        );
        $originalLabels = new SilpoOriginalItemLabelsViewModel(
            $baseCategoryLink !== null ? $baseCategoryLink->shopcategorylabel : null,
            $silpoItem->brand,
            $silpoItem->package,
            $silpoItem->url,
            $silpoItem->country
        );
        $result = new stdClass();
        $result->item = $commonItem;
        $result->originalLabels = $originalLabels;

        return $result;
    }
    private function calorieConverter(?string $cal)
    {
        return $cal != null ? str_replace(",", ".", explode("/", $cal, 2)[0]) : null;
    }
}
class SilpoOriginalItemLabelsViewModel{
    public ?string $categoryLabel;
    public ?string $brandLabel;
    public ?string $packageLabel;
    public ?string $url;
    public ?string $countryLabel;

    public function __construct(
        ?string $categoryLabel = null,
        ?string $brandLabel = null,
        ?string $packageLabel = null,
        ?string $url = null,
        ?string $countryLabel = null
    )
    {
        $this->categoryLabel = $categoryLabel;
        $this->brandLabel = $brandLabel;
        $this->packageLabel = $packageLabel;
        $this->url = $url;
        $this->countryLabel = $countryLabel;
    }
}
