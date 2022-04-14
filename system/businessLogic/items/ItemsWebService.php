<?php

namespace businessLogic\items;

use viewModels\ItemViewModel;
use framework\entities\categories\CategoriesService;
use framework\entities\items\ItemsService;

class ItemsWebService
{
    private ItemsService $_itemsService;
    private CategoriesService $_categoriesService;

    public function __construct()
    {
        $this->_itemsService = new ItemsService();
        $this->_categoriesService = new CategoriesService();
    }

    public function getItemViewModelsByCategory(int $category, int $from, int $to) : array
    {
        $categories = $this->_categoriesService->getCategoriesByParent($category);

        $limit = $to - $from;

        $items = $this->_itemsService->getItemsFromDB([
            'category' => $categories
        ], $from, $limit);

        $result = [];

        foreach ($items as $item) {
            $result[] = new ItemViewModel($item);
        }

        return $result;
    }

    public function getItemViewModelById(int $id) : ItemViewModel
    {
        return new ItemViewModel($this->_itemsService->getItemFromDB($id));
    }
}
