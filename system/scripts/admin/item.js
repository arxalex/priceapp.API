Vue.component('item', {
    template: `
        <div class="row">
            <div class="col-sm-6 row">
                <div class="col-sm-4">
                    <img class="w-100" :src="item.image">
                </div>
                <div class="col-sm-8">
                    <h1>{{ item.label }}</h1>
                    Category: {{ item.category }} - {{ labels.categoryLabel }}<br>
                    Brand: {{ item.brand }} - {{ labels.brandLabel }}<br>
                    Package: {{ item.package }} - {{ labels.packageLabel }}, units: {{ item.units }}, term: {{ item.term }}<br>
                    Barcodes: <span v-for="barcode in item.barcodes">{{ barcode }}, </span><br>
                    Consist: <span v-for="consistId in item.consist">{{ consistId }}, </span> - <span v-for="consistLabel in labels.consistLabels">{{ consistLabel }}, </span><br>
                    Calorie: {{ item.calorie }}, carbohydrates: {{ item.carbohydrates }}, fat: {{ item.fat }}, proteins: {{ item.proteins }}<br>
                    Country: {{ item.additional.country }} - {{ labels.countryLabel }} <br>
                </div>
            </div>
            <div class="col-sm-6 row">
                <div class="col-sm-4">
                    <img class="w-100" :src="currentSimilarItem.image">
                </div>
                <div class="col-sm-8">
                    <h1>{{ currentSimilarItem.label }}</h1>
                    Category: {{ currentSimilarItem.category }} - {{ currentSimilarLabels.categoryLabel }}<br>
                    Brand: {{ currentSimilarItem.brand }} - {{ currentSimilarLabels.brandLabel }}<br>
                    Package: {{ currentSimilarItem.package }} - {{ currentSimilarLabels.packageLabel }}, units: {{ currentSimilarItem.units }}, term: {{ currentSimilarItem.term }}<br>
                    Barcodes: <span v-for="barcode in currentSimilarItem.barcodes">{{ barcode }}, </span><br>
                    Consist: <span v-for="consistId in currentSimilarItem.consist">{{ consistId }}, </span> - <span v-for="consistLabel in currentSimilarLabels.consistLabels">{{ consistLabel }}, </span><br>
                    Calorie: {{ currentSimilarItem.calorie }}, carbohydrates: {{ currentSimilarItem.carbohydrates }}, fat: {{ currentSimilarItem.fat }}, proteins: {{ currentSimilarItem.proteins }}<br>
                    Country: {{ currentSimilarItem.additional.country }} - {{ currentSimilarLabels.countryLabel }}<br>
                </div>
                <div class="">
                    <select v-model="currentSimilarId">
                        <option disabled value="">Chose opinion</option>
                        <option name="item" value="0">New item</option>
                        <option v-for="similarOption in similarOptions" v-bind:value="similarOption.value">{{ similarOption.text }}</option>
                    </select>
                    <button class="btn btn-primary" @click="change">Change destination</button>
                    <button class="btn btn-primary" @click="insertToDB">Insert</button>
                </div>
            </div>
        </div>
    `,
    data() {
        return {
            currentSimilarItem: {
                id: 0,
                label: "",
                category: 0,
                package: 0,
                brand: 0,
                calorie: 0,
                carbohydrates: 0,
                fat: 0,
                proteins: 0,
                barcodes: [],
                consist: [],
                term: 0,
                units: 0,
                additional: [],
                image: ""
            },
            currentSimilarLabels: {
                categoryLabel: "",
                brandLabel: "",
                packageLabel: "",
                consistLabels: [],
                countryLabel: ""
            },
            currentSimilarId: 0,
            similarOptions: []
        }
    },
    mounted() {
        var i = 1;
        this.similarItems.forEach(element => {
            this.similarOptions.push({ text: element.label, value: i++ });
        });
    },
    methods: {
        change: function () {
            if (this.currentSimilarId == 0) {
                this.insertToDB();
            } else {
                this.currentSimilarItem = this.similarItems[this.currentSimilarId - 1];
                this.currentSimilarLabels = this.similarLabels[this.currentSimilarId - 1];
            }
        },
        insertToDB: function () {

        },
        new_item: function () {

        }
    },
    props: {
        item: {
            type: Object
        },
        similarItems: {
            type: Array
        },
        labels: {
            type: Object
        },
        similarLabels: {
            type: Array
        }
    },
    computed: {
    },
});