<?php

namespace framework\shops\atb;

use framework\database\ListHelper;
use framework\entities\brands\BrandsService;
use framework\entities\categories\CategoriesService;
use framework\entities\items\Item;
use framework\entities\categories_link\CategoriesLinkService;
use framework\entities\countries\CountriesService;
use framework\entities\items_link\ItemsLinkService;
use framework\entities_proxy\items\AtbItemsService;
use framework\shops\atb\AtbItemModel;
use stdClass;

class AtbItemsGetter
{
    private CategoriesService $_categoriesService;
    private CategoriesLinkService $_categoriesLinkService;
    private BrandsService $_brandService;
    private CountriesService $_countriesService;
    private ItemsLinkService $_itemsLinkService;
    private AtbItemsService $_atbItemsService;

    public function __construct()
    {
        $this->_categoriesLinkService = new CategoriesLinkService();
        $this->_categoriesService = new CategoriesService();
        $this->_brandService = new BrandsService();
        $this->_countriesService = new CountriesService();
        $this->_itemsLinkService = new ItemsLinkService();
        $this->_atbItemsService = new AtbItemsService();
    }
    public function get(int $categotyId, int $from = 0, int $to = 25): array
    {
        $resultItems = $this->_atbItemsService->getItemsFromDB(['category' => [$categotyId]], $from, $to - $from);
        $inTableItems = ListHelper::getColumn($this->_itemsLinkService->getItemsFromDB([
            'inshopid' => ListHelper::getColumn($resultItems, 'id'),
            'shopid' => [3]
        ]), 'inshopid');
        $handledResult = $resultItems;
        if (count($inTableItems) > 0) {
            foreach ($handledResult as $key => $value) {
                if(in_array($value->id, $inTableItems)){
                    unset($handledResult[$key]);
                }
            }
        }
        $items = [];
        foreach ($handledResult as $value) {
            $items[] = new AtbItemModel(
                $value->id,
                $value->label,
                $value->image,
                $value->category,
                $value->brand,
                $value->country,
                "https://zakaz.atbmarket.com/product/1154/" . $value->internalid
            );
        }
        return $items;
    }
    public function convertFromAtbToCommonModel(AtbItemModel $atbItem)
    {
        $baseCategoryLinks = $this->_categoriesLinkService->getItemsFromDB([
            'categoryshopid' => [$atbItem->shopcategoryid],
            'shopid' => [3]
        ]);
        $baseCategoryLink = count($baseCategoryLinks) > 0 ? $baseCategoryLinks[0] : null;
        $baseCategoryId = $baseCategoryLink !== null ? $baseCategoryLink->categoryid : null;

        $baseCategory = $baseCategoryId !== null ? $this->_categoriesService->getItemFromDB($baseCategoryId) : null;
        $category = $this->_categoriesService->getCategoryByName($atbItem->label, $baseCategory);
        $brand = $this->_brandService->getBrand($atbItem->brand);
        $country = $this->_countriesService->getCountry($atbItem->country);
        $commonItem = new Item(
            null,
            $atbItem->label,
            $atbItem->image,
            $category->id,
            $brand->id,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            [
                'country' => $country->id
            ]
        );
        $originalLabels = new AtbOriginalItemLabelsViewModel(
            $baseCategoryLink !== null ? $baseCategoryLink->shopcategorylabel : null,
            $atbItem->brand,
            null,
            $atbItem->url,
            $atbItem->country,
            $atbItem->inshopid,
            3
        );
        $result = new stdClass();
        $result->item = $commonItem;
        $result->originalLabels = $originalLabels;

        return $result;
    }
}
class AtbOriginalItemLabelsViewModel{
    public ?string $categoryLabel;
    public ?string $brandLabel;
    public ?string $packageLabel;
    public ?string $url;
    public ?string $countryLabel;
    public ?int $inShopId;
    public ?int $shopId;

    public function __construct(
        ?string $categoryLabel = null,
        ?string $brandLabel = null,
        ?string $packageLabel = null,
        ?string $url = null,
        ?string $countryLabel = null,
        ?int $inShopId = null,
        ?int $shopId = null
    )
    {
        $this->categoryLabel = $categoryLabel;
        $this->brandLabel = $brandLabel;
        $this->packageLabel = $packageLabel;
        $this->url = $url;
        $this->countryLabel = $countryLabel;
        $this->inShopId = $inShopId;
        $this->shopId = $shopId;
    }
}
