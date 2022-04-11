<?php

namespace endpoint\items;

use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\entities\items\ItemsService;
use framework\database\StringHelper;
use framework\entities\categories\CategoriesService;
use framework\shops\silpo\SilpoItemsGetter;
use framework\viewModels\ItemViewModel;

class GetItems extends BaseEndpointBuilder
{
    private ItemsService $_itemsService;
    private SilpoItemsGetter $_silpoItemsGetter;
    private CategoriesService $_categoriesService;
    public function __construct()
    {
        parent::__construct();
        $this->_itemsService = new ItemsService();
        $this->_silpoItemsGetter = new SilpoItemsGetter();
        $this->_categoriesService = new CategoriesService();
    }
    public function defaultParams()
    {
        return [
            'id' => null,
            'method' => 'byId',
            'source' => 0,
            'name' => null,
            'category' => null,
            'from' => 0,
            'to' => 25,
            'cookie' => []
        ];
    }
    public function build()
    {
        $this->_usersService->unavaliableRequest($this->getParam('cookie'), 1);
        if ($this->getParam('source') === 0) {
            if ($this->getParam('method') == 'byId') {
                $this->_usersService->unavaliableRequest($this->getParam('cookie'));
                return $this->_itemsService->getItemFromDB($this->getParam('id'));
            } elseif ($this->getParam('method') == 'byNameAndCategory') {
                $this->_usersService->unavaliableRequest($this->getParam('cookie'));
                $label = $this->getParam('name');
                $nameArr = StringHelper::nameToKeywords($this->getParam('name'));
                $result = $this->_itemsService->getItemsFromDB([
                    'label_like' => $nameArr,
                    'category' => [],
                ]);
                $rate = StringHelper::rateItemsByKeywords($label, array_column($result, 'label'));
                return $this->_itemsService->orderItemsByRate($result, $rate, 5);
            } elseif ($this->getParam('method') == 'viewModelByCategory' && $this->getParam('category') != 0) {
                $categories = $this->_categoriesService->getCategoriesByParent($this->getParam('category'));
                $items = $this->_itemsService->getItemsFromDB([
                    'category' => $categories
                ]);

                $result = [];

                foreach($items as $item){
                    $result[] = new ItemViewModel($item);
                }

                return $result;
            }
        } elseif($this->getParam('source') === 1){
            $this->_usersService->unavaliableRequest($this->getParam('cookie'));
            $silpoItemsModels = $this->_silpoItemsGetter->get($this->getParam('category'), $this->getParam('from'), $this->getParam('to'));
            $items = [];
            foreach($silpoItemsModels as $silpoItemModel){
                $items[] = $this->_silpoItemsGetter->convertFromSilpoToCommonModel($silpoItemModel);
            }
            return $items;
        }
    }
}
