<?php

namespace endpoint\atb;

use atb\AtbItems;
use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\database\NumericHelper;
use framework\database\Request;
use framework\entities\constants\ConstantsService;
use framework\entities\categories\AtbCategoriesService;
use framework\entities\filials\AtbFilialsService;

class LoadPrices extends BaseEndpointBuilder
{
    private AtbItems $_atbItems;
    private AtbCategoriesService $_atbCategoriesService;
    private AtbFilialsService $_atbFilialsService;
    private ConstantsService $_constantsService;

    public function __construct()
    {
        parent::__construct();
        $this->_atbItems = new AtbItems();
        $this->_atbCategoriesService = new AtbCategoriesService();
        $this->_atbFilialsService = new AtbFilialsService();
        $this->_constantsService = new ConstantsService();
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

        $dateToday = date("Y-m-d");
        $lastUpdatedDate = ($this->_constantsService->getItemsFromDB(['label' => ['ATB_LAST_SCANNED_LASTUPDATEDATE']]))[0];

        if ($lastUpdatedDate->value == $dateToday) {
            return 0;
        } else {
            $lastFilial = ($this->_constantsService->getItemsFromDB(['label' => ['ATB_LAST_SCANNED_FILIALID']]))[0];
            $lastCategory = ($this->_constantsService->getItemsFromDB(['label' => ['ATB_LAST_SCANNED_CATEGORYID']]))[0];

            if(NumericHelper::toInt($lastFilial->value) == 0 && NumericHelper::toInt($lastCategory->value) == 0){
                (new Request("UPDATE `pa_prices` SET `quantity`='0'"))->execute();
            }

            $filials = $this->_atbFilialsService->getItemsFromDB();
            $categories = $this->_atbCategoriesService->getItemsFromDB(['parent' => [null]]);

            $count = 0;
            for ($i = NumericHelper::toInt($lastFilial->value); $i < count($filials); $i++) {
                for ($j = NumericHelper::toInt($lastCategory->value); $j < count($categories); $j++) {
                    $count += $this->_atbItems->loadItemsPricesByCategoryAndFilial($categories[$j]->internalid, $filials[$i]->inshopid);

                    $lastCategory->value = $j + 1;
                    $this->_constantsService->updateItemInDB($lastCategory);
                }
                $lastCategory->value = 0;
                $this->_constantsService->updateItemInDB($lastCategory);
                $lastFilial->value = $i + 1;
                $this->_constantsService->updateItemInDB($lastFilial);
            }
            $lastFilial->value = 0;
            $this->_constantsService->updateItemInDB($lastFilial);
            $lastUpdatedDate->value = $dateToday;
            $this->_constantsService->updateItemInDB($lastUpdatedDate);

            return $count;
        }
    }
}
