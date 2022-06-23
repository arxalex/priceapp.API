<?php

namespace endpoint\atb;

use atb\AtbItems;
use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\database\NumericHelper;
use framework\entities\constants\ConstantsService;
use framework\entities\categories\AtbCategoriesService;
use framework\entities\filials\AtbFilialsService;

class LoadPricesProcess extends BaseEndpointBuilder
{
    private AtbItems $_atbItems;
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
            'tasks' => [],
            'key' => null,
            'cookie' => []
        ];
    }
    public function build()
    {
        $this->_usersService->unavaliableRequest($this->getParam('cookie'));
        
        $tasksFilial = $this->getParam('tasks');
        $key = $this->getParam('key');

        $i2 = $key + 1;
        $lastCategoryForFilial = $this->_constantsService->getItemsFromDB(['label' => ["ATB_LAST_SCANNED_CATEGORYID_$i2"]])[0];
        foreach($tasksFilial as $j => $task){
            $this->_atbItems->loadItemsPricesByCategoryAndFilial($task['args'][0], $task['args'][1]);
            $lastCategoryForFilial->value = $j + 1;
            $this->_constantsService->updateItemInDB($lastCategoryForFilial);

            $inProcessCount = ($this->_constantsService->getItemsFromDB(['label' => ['ATB_INPROCESSING_COUNT']]))[0];
            $inProcessCount->value = NumericHelper::toInt($inProcessCount->value) + 1;
            $this->_constantsService->updateItemInDB($inProcessCount);
        }

        $lastCategoryForFilial->value = date("Y-m-d");
        $this->_constantsService->updateItemInDB($lastCategoryForFilial);

        return 0;
    }
}
