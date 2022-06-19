<?php

namespace endpoint\atb;

use atb\AtbItems;
use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\entities\constants\ConstantsService;
use framework\entities\categories\AtbCategoriesService;
use framework\database\NumericHelper;

class LoadProducts extends BaseEndpointBuilder
{
    private AtbCategoriesService $_atbCategoriesService;
    private ConstantsService $_constantsService;
    private AtbItems $_atbItems;
    public function __construct()
    {
        parent::__construct();
        $this->_atbCategoriesService = new AtbCategoriesService();
        $this->_constantsService = new ConstantsService();
        $this->_atbItems = new AtbItems();
    }
    public function defaultParams()
    {
        return [
            'cookie' => []
        ];
    }
    public function build()
    {
        $this->_usersService->unavaliableRequest($this->getParam('cookie'));

        $lastCategory = ($this->_constantsService->getItemsFromDB(['label' => ['ATB_LAST_LOADPRODUCTS_CATEGORYID']]))[0];
        $categories = $this->_atbCategoriesService->getItemsFromDB(['parent' => [null]]);
        $count = 0;

        for ($j = NumericHelper::toInt($lastCategory->value); $j < count($categories); $j++) {
            $count += $this->_atbItems->loadItemsByCategoryAndFilial($categories[$j]->internalid);

            $lastCategory->value = $j + 1;
            $this->_constantsService->updateItemInDB($lastCategory);
        }
        $lastCategory->value = 0;
        $this->_constantsService->updateItemInDB($lastCategory);

        return $count;
    }
}
