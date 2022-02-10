Vue.component('item', {
    template: `
        <div class="row">
            <div class="col-sm-6 row">
                <div class="col-sm-4">
                    <img class="w-100" :src="item.image">
                </div>
                <div class="col-sm-8">
                    <h1>{{ item.label }}</h1>
                    Category: {{ item.category }} - {{ item.categoryLabel }}<br>
                    Brand: {{ item.brand }} - {{ item.brandLabel }}<br>
                    Package: {{ item.package }} - {{ item.packageLabel }}, units: {{ item.units }}, term: {{ item.term }}<br>
                    Barcodes: <span v-for="barcode in item.barcodes">{{ barcode }}, </span><br>
                    Consist: <span v-for="cons in item.consist">{{ cons }}, </span> - <span v-for="cons in item.consistLabels">{{ cons }}, </span><br>
                    Calorie: {{ item.calorie }}, carbohydrates: {{ item.carbohydrates }}, fat: {{ item.fat }}, proteins: {{ item.proteins }}<br>
                    Country: {{ item.additional.country }} - {{ item.additionalLabels.country }}<br>
                    Simid: {{ item.itemSimId }}
                </div>
            </div>
            <div class="col-sm-6 row">
                <div class="col-sm-4">
                    <img class="w-100" :src="item.image">
                </div>
                <div class="col-sm-8">
                    <h1>{{ item.itemSim.label }}</h1>
                    Category: {{ item.itemSim.category }} - {{ item.itemSim.categoryLabel }}<br>
                    Brand: {{ item.itemSim.brand }} - {{ item.itemSim.brandLabel }}<br>
                    Package: {{ item.itemSim.package }} - {{ item.itemSim.packageLabel }}, units: {{ item.itemSim.units }}, term: {{ item.itemSim.term }}<br>
                    Barcodes: <span v-for="barcode in item.itemSim.barcodes">{{ barcode }}, </span><br>
                    Consist: <span v-for="cons in item.itemSim.consist">{{ cons }}, </span> - <span v-for="cons in item.itemSim.consistLabels">{{ cons }}, </span><br>
                    Calorie: {{ item.itemSim.calorie }}, carbohydrates: {{ item.itemSim.carbohydrates }}, fat: {{ item.itemSim.fat }}, proteins: {{ item.itemSim.proteins }}<br>
                    Country: {{ item.itemSim.additional.country }} - {{ item.itemSim.additionalLabels.country }}<br>
                </div>
                <div class="">
                    <input v-model="destination" type="number" placeholder="id">
                    <button class="btn btn-primary" @click="change">Change destination</button>
                    <button class="btn btn-primary" @click="new_item">New item</button>
                </div>
            </div>
        </div>
    `,
    methods: {
        change: function(){

        },
        new_item: function(){

        }
    },
    props: ['item'],
    computed: {
    },
});
export function register() {
    
}