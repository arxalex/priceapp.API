<?php

namespace render\defaultBuild;

use render\defaultBuild\BaseRenderBuilder;

class Header extends BaseRenderBuilder
{
    public function defaultParams()
    {   
        return [
            'header_title' => 'PriceApp',
            'links' => [
                '<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.0.2/dist/css/bootstrap.min.css" rel="stylesheet" crossorigin="anonymous">',
                '<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.7.1/font/bootstrap-icons.css">',
                '<link href="/system/style/default/style.css" rel="stylesheet">'
            ],
        ];
    }
    public function build()
    {
        $paramsString = '';
        foreach($this->getParam('links') as $value){
            $paramsString .= $value;
        }
        return '<!DOCTYPE html>
        <html lang="en">
        <head>
            <meta charset="UTF-8">
            <meta http-equiv="X-UA-Compatible" content="IE=edge">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <title>' . $this->getParam('header_title') . '</title>
            '. $paramsString .'
        </head>
        <body>
        <div id="page-wrapper" class="container-fluid px-0">
            <div class="navbar navbar-expand-sm navbar-light bg-light mb-3">
                <div class="container-fluid">

                    <div class="mx-auto text-center d-block">
                        <a class="navbar-brand col-12 me-0" href="./">'.
                            //'<img class="d-inline-block align-text-top" height="30px" src="secret-santa.svg">'.
                            '<span class="small">PriceApp</span>
                        </a>
                        <span class="d-inline small">by</span>
                        <a href="../">
                            <img class="d-inline" height="25px" src="../../../arxlogo.svg">
                        </a>
                    </div>

                </div>
            </div>
            <div class="container">';
    }
}
