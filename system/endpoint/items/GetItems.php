<?php

namespace endpoint\items;

use businessLogic\items\ItemsWebService;
use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\entities\items\ItemsService;
use framework\database\StringHelper;
use framework\shops\atb\AtbItemsGetter;
use framework\shops\fora\ForaItemsGetter;
use framework\shops\silpo\SilpoItemsGetter;

class GetItems extends BaseEndpointBuilder
{
    private ItemsService $_itemsService;
    private SilpoItemsGetter $_silpoItemsGetter;
    private ForaItemsGetter $_foraItemsGetter;
    private AtbItemsGetter $_atbItemsGetter;
    private ItemsWebService $_itemsWebService;
    public function __construct()
    {
        parent::__construct();
        $this->_itemsService = new ItemsService();
        $this->_silpoItemsGetter = new SilpoItemsGetter();
        $this->_itemsWebService = new ItemsWebService();        
        $this->_foraItemsGetter = new ForaItemsGetter();
        $this->_atbItemsGetter = new AtbItemsGetter();
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
            'xCord' => null,
            'yCord' => null,
            'radius' => null,
            'search' => null,
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
                return $this->_itemsWebService->getItemViewModelsByCategory(
                    $this->getParam('category'),
                    $this->getParam('from'),
                    $this->getParam('to')
                );
            } elseif (
                $this->getParam('method') == 'viewModelByCategoryAndLocation'
                && $this->getParam('category') != 0
                && $this->getParam('xCord') != null
                && $this->getParam('yCord') != null
                && $this->getParam('radius') != null
            ) {
                return $this->_itemsWebService->getItemViewModelsByCategory(
                    $this->getParam('category'),
                    $this->getParam('from'),
                    $this->getParam('to'),
                    $this->getParam('xCord'),
                    $this->getParam('yCord'),
                    $this->getParam('radius')
                );
            } elseif ($this->getParam('method') == 'viewModelBySearch' && $this->getParam('search') != "") {
                return $this->_itemsWebService->getItemViewModelsBySearch(
                    $this->getParam('search'),
                    $this->getParam('from'),
                    $this->getParam('to')
                );
            } elseif (
                $this->getParam('method') == 'viewModelBySearchAndLocation'
                && $this->getParam('search') != ""
                && $this->getParam('xCord') != null
                && $this->getParam('yCord') != null
                && $this->getParam('radius') != null
            ) {
                return $this->_itemsWebService->getItemViewModelsBySearch(
                    $this->getParam('search'),
                    $this->getParam('from'),
                    $this->getParam('to'),
                    $this->getParam('xCord'),
                    $this->getParam('yCord'),
                    $this->getParam('radius')
                );
            }
        } elseif ($this->getParam('source') === 1) {
            $this->_usersService->unavaliableRequest($this->getParam('cookie'));
            $silpoItemsModels = $this->_silpoItemsGetter->get($this->getParam('category'), $this->getParam('from'), $this->getParam('to'));
            $items = [];
            foreach ($silpoItemsModels as $silpoItemModel) {
                $items[] = $this->_silpoItemsGetter->convertFromSilpoToCommonModel($silpoItemModel);
            }
            return $items;
        } elseif ($this->getParam('source') === 2) {
            $this->_usersService->unavaliableRequest($this->getParam('cookie'));
            $foraItemsModels = $this->_foraItemsGetter->get($this->getParam('category'), $this->getParam('from'), $this->getParam('to'));
            $items = [];
            foreach ($foraItemsModels as $foraItemModel) {
                $items[] = $this->_foraItemsGetter->convertFromForaToCommonModel($foraItemModel);
            }
            return $items;
        } elseif ($this->getParam('source') === 3) {
            $this->_usersService->unavaliableRequest($this->getParam('cookie'));
            $atbItemsModels = $this->_atbItemsGetter->get($this->getParam('category'), $this->getParam('from'), $this->getParam('to'));
            $items = [];
            foreach ($atbItemsModels as $atbItemModel) {
                $items[] = $this->_atbItemsGetter->convertFromAtbToCommonModel($atbItemModel);
            }
            return $items;
        }
    }
}
