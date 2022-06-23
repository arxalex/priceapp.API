<?php

namespace endpoint\atb;

use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\database\NumericHelper;
use framework\database\Request;
use framework\entities\constants\ConstantsService;
use framework\entities\categories\AtbCategoriesService;
use framework\entities\constants\Constant;
use framework\entities\filials\AtbFilialsService;
use stdClass;

class LoadPrices extends BaseEndpointBuilder
{
    private AtbCategoriesService $_atbCategoriesService;
    private AtbFilialsService $_atbFilialsService;
    private ConstantsService $_constantsService;

    public function __construct()
    {
        parent::__construct();
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
            $maxNumberOfThreads = NumericHelper::toInt(($this->_constantsService->getItemsFromDB(['label' => ['ATB_MAX_COUNT_OF_THREADS']]))[0]->value);
            $currentNumberOfThreads = 0;

            $inProcessCount = ($this->_constantsService->getItemsFromDB(['label' => ['ATB_INPROCESSING_COUNT']]))[0];
            $inProcessCount->value = NumericHelper::toInt($inProcessCount->value) + 1;
            $this->_constantsService->updateItemInDB($inProcessCount);

            if(NumericHelper::toInt($inProcessCount->value) == 0){
                (new Request("UPDATE `pa_prices` SET `quantity`='0'"))->execute();
            }

            $filials = $this->_atbFilialsService->getItemsFromDB();
            $categories = $this->_atbCategoriesService->getItemsFromDB(['parent' => [null]]);

            $count = 0;
            $tasks = [];
            for ($i = 0; $i < count($filials) && $currentNumberOfThreads < $maxNumberOfThreads; $i++) {
                $i2 = $i + 1;
                $lastCategoryForFilialArr = $this->_constantsService->getItemsFromDB(['label' => ["ATB_LAST_SCANNED_CATEGORYID_$i2"]]);

                if (count($lastCategoryForFilialArr) > 0) {
                    $lastCategoryForFilial = $lastCategoryForFilialArr[0];
                } else {
                    $lastCategoryForFilial = new Constant();
                    $lastCategoryForFilial->label = "ATB_LAST_SCANNED_CATEGORYID_$i2";
                    $lastCategoryForFilial->value = 0;

                    $this->_constantsService->insertItemToDB($lastCategoryForFilial);
                }

                if ($lastCategoryForFilial->value == $dateToday) {
                    continue;
                }

                $tasks[$i] = [];

                for ($j = NumericHelper::toInt($lastCategoryForFilial->value); $j < count($categories); $j++) {
                    $tasks[$i][$j] = [
                        'args' => [
                            $categories[$j]->internalid,
                            $filials[$i]->inshopid
                        ]
                    ];
                }

                $currentNumberOfThreads++;
            }
            foreach ($tasks as $key => $tasksFilial) {
                $requestArr = new stdClass();
                $requestArr->key = $key;
                $requestArr->tasks = $tasksFilial; 
                $curl = curl_init();

                curl_setopt_array($curl, array(
                    CURLOPT_URL => 'http://localhost/be/atb/load_prices_process',
                    CURLOPT_RETURNTRANSFER => true,
                    CURLOPT_TIMEOUT => 1,
                    CURLOPT_CUSTOMREQUEST => 'POST',
                    CURLOPT_POSTFIELDS => json_encode($requestArr),
                    CURLOPT_HTTPHEADER => array(
                        'Content-Type: application/json',
                        'Cookie: userid=1; token=lvRiyHas333xJC9eah1Wk3WxyFvm8fgi; token_expires=4294967295'
                    ),
                ));

                curl_exec($curl);
                curl_close($curl);
            }
            sleep(1100);

            return $count;
        }
    }
}
