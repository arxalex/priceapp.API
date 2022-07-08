<?php

namespace businessLogic\items;

use framework\database\ListHelper;
use viewModels\ItemViewModel;
use framework\entities\categories\CategoriesService;
use framework\entities\filials\FilialsService;
use framework\entities\items\ItemsService;
use framework\database\StringHelper;

class ItemsWebService
{
    private ItemsService $_itemsService;
    private CategoriesService $_categoriesService;
    private FilialsService $_filialsService;

    public function __construct()
    {
        $this->_itemsService = new ItemsService();
        $this->_categoriesService = new CategoriesService();
        $this->_filialsService = new FilialsService();
    }

    public function getItemViewModelsByCategory(
        int $category,
        int $from,
        int $to,
        ?float $xCord = null,
        ?float $yCord = null,
        ?float $radius = null
    ): array {
        $categories = $this->_categoriesService->getCategoriesByParent($category);

        $limit = $to - $from;

        $result = [];

        if ($xCord === null || $yCord === null || $radius === null) {
            $items = $this->_itemsService->getItemsFromDB([
                'category' => $categories
            ], $from, $limit);
            foreach ($items as $item) {
                $preResult = new ItemViewModel($item);
                if ($preResult->priceMin !== null && $preResult->priceMax !== null) {
                    $result[] = $preResult;
                }
            }
        } else {
            $filials = $this->_filialsService->getFilialsByCord($xCord, $yCord, $radius);
            $items = $this->_itemsService->getItemsFromDBByFilials($categories, ListHelper::getColumn($filials, 'id'), $from, $limit);
            foreach ($items as $item) {
                $preResult = new ItemViewModel($item, $filials);
                if ($preResult->priceMin !== null && $preResult->priceMax !== null) {
                    $result[] = $preResult;
                }
            }
        }

        return $result;
    }

    public function getItemViewModelById(
        int $id,
        ?float $xCord = null,
        ?float $yCord = null,
        ?float $radius = null
    ): ItemViewModel {
        if ($xCord === null || $yCord === null || $radius === null) {
            return new ItemViewModel($this->_itemsService->getItemFromDB($id));
        } else {
            $filials = $this->_filialsService->getFilialsByCord($xCord, $yCord, $radius);
            return new ItemViewModel($this->_itemsService->getItemFromDB($id), $filials);
        }
    }

    public function getItemViewModelsBySearch(
        string $search,
        int $from,
        int $to,
        ?float $xCord = null,
        ?float $yCord = null,
        ?float $radius = null
    ): array {
        $labelArr = StringHelper::nameToKeywords($search);
        $itemsFromDB = $this->_itemsService->getItemsFromDB([
            'label_like' => $labelArr
        ]);

        $rates = StringHelper::rateItemsByKeywords($search, array_column($itemsFromDB, 'label'));

        $itemsNotTrimmed = $this->_itemsService->orderItemsByRate($itemsFromDB, $rates);

        $items = [];

        for ($i = $from; $i < $to && $i < count($itemsNotTrimmed); $i++) {
            $items[] = $itemsNotTrimmed[$i];
        }

        $result = [];

        if ($xCord === null || $yCord === null || $radius === null) {
            foreach ($items as $item) {
                $preResult = new ItemViewModel($item);
                if ($preResult->priceMin !== null && $preResult->priceMax !== null) {
                    $result[] = $preResult;
                }
            }
        } else {
            $filials = $this->_filialsService->getFilialsByCord($xCord, $yCord, $radius);
            foreach ($items as $item) {
                $preResult = new ItemViewModel($item, $filials);
                if ($preResult->priceMin !== null && $preResult->priceMax !== null) {
                    $result[] = $preResult;
                }
            }
        }

        return $result;
    }
}
