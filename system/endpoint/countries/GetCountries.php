<?php

namespace endpoint\countries;

use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\entities\countries\CountriesService;

class GetCountries extends BaseEndpointBuilder
{
    private CountriesService $_countriesService;
    public function __construct()
    {
        parent::__construct(); 
        $this->_countriesService = new CountriesService();
    }
    public function defaultParams()
    {
        return [
            'method' => "GetCountryByLabel",
            'label' => ""
        ];
    }
    public function build()
    {
        if ($this->getParam('method') == "GetCountryByLabel") {
            $label = $this->getParam('label');
            return $this->_countriesService->getCountry($label);
        }
    }
}
